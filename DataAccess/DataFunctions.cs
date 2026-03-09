using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectPlan.Models;

namespace ProjectPlan.DataAccess;

public static class DataFunctions
{
    public static async Task<IReadOnlyList<Project>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        await using var db = new Context();
        return await db.Projects
            .OrderByDescending(p => p.CreationDate)
            .ThenBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public static async Task<Project?> GetProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        await using var db = new Context();
        return await db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
    }

    public static async Task<IReadOnlyList<Column>> GetBoardColumnsAsync(int projectId, CancellationToken cancellationToken = default)
    {
        await using var db = new Context();

        var columns = await db.Columns
            .Where(c => c.IdProject == projectId)
            .Include(c => c.Cards)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Deterministic ordering for UI.
        return columns
            .OrderBy(c => c.Id)
            .Select(c =>
            {
                c.Cards = c.Cards.OrderBy(card => card.Id).ToList();
                return c;
            })
            .ToList();
    }

    public static async Task<Project> CreateProjectAsync(
        string name,
        string? description,
        string? thumbnailPath,
        string? background,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name is required", nameof(name));

        await using var db = new Context();

        var maxId = await db.Projects.MaxAsync(p => (int?)p.Id, cancellationToken);
        var nextId = (maxId ?? 0) + 1;

        var project = new Project
        {
            Id = nextId,
            Name = name.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Thumbnail = string.IsNullOrWhiteSpace(thumbnailPath) ? null : thumbnailPath,
            Background = string.IsNullOrWhiteSpace(background) ? null : background,
            CreationDate = DateTime.UtcNow,
            Mute = false,
            Columns = new List<Column>()
            {
                new Column {Name = "Backlog"},
                new Column {Name = "To Do"},
                new Column {Name = "In Progress"},
                new Column {Name = "Done"},
            }
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(cancellationToken);

        return project;
    }

    public static async Task<Project?> UpdateProjectAsync(
        int projectId,
        string name,
        string? description,
        string? thumbnailPath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name is required", nameof(name));

        await using var db = new Context();

        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        if (project is null)
            return null;

        project.Name = name.Trim();
        project.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        project.Thumbnail = string.IsNullOrWhiteSpace(thumbnailPath) ? null : thumbnailPath;

        await db.SaveChangesAsync(cancellationToken);
        return project;
    }

    public static async Task<bool> DeleteProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        await using var db = new Context();

        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        if (project is null)
            return false;

        var columnIds = await db.Columns
            .Where(c => c.IdProject == projectId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        if (columnIds.Count > 0)
        {
            var cardIds = await db.Cards
                .Where(c => columnIds.Contains(c.IdColumn))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            if (cardIds.Count > 0)
            {
                var histories = await db.CardHistories
                    .Where(h => cardIds.Contains(h.IdCard))
                    .ToListAsync(cancellationToken);
                if (histories.Count > 0)
                    db.CardHistories.RemoveRange(histories);

                var cards = await db.Cards
                    .Where(c => cardIds.Contains(c.Id))
                    .ToListAsync(cancellationToken);
                if (cards.Count > 0)
                    db.Cards.RemoveRange(cards);
            }

            var columns = await db.Columns
                .Where(c => columnIds.Contains(c.Id))
                .ToListAsync(cancellationToken);
            if (columns.Count > 0)
                db.Columns.RemoveRange(columns);
        }

        db.Projects.Remove(project);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public static async Task<Column> CreateColumnAsync(
        int projectId,
        string name,
        string? color,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Column name is required", nameof(name));

        await using var db = new Context();

        var maxId = await db.Columns.MaxAsync(c => (int?)c.Id, cancellationToken);
        var nextId = (maxId ?? 0) + 1;

        var column = new Column
        {
            Id = nextId,
            IdProject = projectId,
            Name = name.Trim(),
            Color = string.IsNullOrWhiteSpace(color) ? null : color,
        };

        db.Columns.Add(column);
        await db.SaveChangesAsync(cancellationToken);

        return column;
    }

    public static async Task<bool> DeleteColumnAsync(int columnId, CancellationToken cancellationToken = default)
    {
        await using var db = new Context();

        var column = await db.Columns.FirstOrDefaultAsync(c => c.Id == columnId, cancellationToken);
        if (column is null)
            return false;

        var cardIds = await db.Cards
            .Where(c => c.IdColumn == columnId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        if (cardIds.Count > 0)
        {
            var histories = await db.CardHistories
                .Where(h => cardIds.Contains(h.IdCard))
                .ToListAsync(cancellationToken);
            if (histories.Count > 0)
                db.CardHistories.RemoveRange(histories);

            var cards = await db.Cards
                .Where(c => cardIds.Contains(c.Id))
                .ToListAsync(cancellationToken);
            if (cards.Count > 0)
                db.Cards.RemoveRange(cards);
        }

        db.Columns.Remove(column);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public static async Task<Card> CreateCardAsync(
        int columnId,
        string title,
        string? description,
        string? color,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Card title is required", nameof(title));

        await using var db = new Context();

        var maxId = await db.Cards.MaxAsync(c => (int?)c.Id, cancellationToken);
        var nextId = (maxId ?? 0) + 1;

        var card = new Card
        {
            Id = nextId,
            IdColumn = columnId,
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Color = string.IsNullOrWhiteSpace(color) ? null : color,
            Notify = false,
            StartDate = null,
            EndDate = null,
        };

        db.Cards.Add(card);
        await db.SaveChangesAsync(cancellationToken);

        return card;
    }

    public static async Task<Card?> UpdateCardAsync(
        int cardId,
        string title,
        string? description,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Card title is required", nameof(title));

        await using var db = new Context();

        var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);
        if (card is null)
            return null;

        card.Title = title.Trim();
        card.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        await db.SaveChangesAsync(cancellationToken);
        return card;
    }

    public static async Task<bool> MoveCardToColumnAsync(
        int cardId,
        int newColumnId,
        CancellationToken cancellationToken = default)
    {
        await using var db = new Context();

        var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);
        if (card is null)
            return false;

        // Basic validation: ensure the target column exists.
        var columnExists = await db.Columns.AnyAsync(c => c.Id == newColumnId, cancellationToken);
        if (!columnExists)
            return false;

        card.IdColumn = newColumnId;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public static async Task<bool> DeleteCardAsync(int cardId, CancellationToken cancellationToken = default)
    {
        await using var db = new Context();

        // Delete history first (FK constraint).
        var histories = await db.CardHistories
            .Where(h => h.IdCard == cardId)
            .ToListAsync(cancellationToken);

        if (histories.Count > 0)
            db.CardHistories.RemoveRange(histories);

        var card = await db.Cards.FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);
        if (card is null)
            return false;

        db.Cards.Remove(card);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
