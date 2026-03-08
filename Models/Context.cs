using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectPlan.Models;

public partial class Context : DbContext
{
    private const string SqlTypeVarchar = "VARCHAR";
    private const string SqlTypeDateTime = "DATETIME";
    private const string SqlTypeBoolean = "BOOLEAN";

    public static async Task<bool> EnsureDatabaseCreated() => await new Context().Database.EnsureCreatedAsync();

    public Context()
    {
    }

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<CardHistory> CardHistories { get; set; }

    public virtual DbSet<Column> Columns { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        DbLocation.EnsureDirectoryExists();
        optionsBuilder.UseSqlite(DbLocation.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.ToTable("Card");

            entity.HasIndex(e => e.Id, "IX_Card_id").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Color)
                .HasColumnType(SqlTypeVarchar)
                .HasColumnName("color");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType(SqlTypeDateTime)
                .HasColumnName("end_date");
            entity.Property(e => e.IdColumn).HasColumnName("id_column");
            entity.Property(e => e.Notify)
                .IsRequired()
                .HasDefaultValueSql("false")
                .HasColumnType(SqlTypeBoolean)
                .HasColumnName("notify");
            entity.Property(e => e.StartDate)
                .HasColumnType(SqlTypeDateTime)
                .HasColumnName("start_date");
            entity.Property(e => e.Title)
                .HasColumnType(SqlTypeVarchar)
                .HasColumnName("title");

            entity.HasOne(d => d.IdColumnNavigation).WithMany(p => p.Cards)
                .HasForeignKey(d => d.IdColumn)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CardHistory>(entity =>
        {
            entity.ToTable("CardHistory");

            entity.HasIndex(e => e.Id, "IX_CardHistory_id").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IdCard).HasColumnName("id_card");
            entity.Property(e => e.Text).HasColumnName("text");
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType(SqlTypeDateTime)
                .HasColumnName("update_time");

            entity.HasOne(d => d.IdCardNavigation).WithMany(p => p.CardHistories)
                .HasForeignKey(d => d.IdCard)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Column>(entity =>
        {
            entity.ToTable("Column");

            entity.HasIndex(e => e.Id, "IX_Column_id").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Color)
                .HasColumnType(SqlTypeVarchar)
                .HasColumnName("color");
            entity.Property(e => e.IdProject).HasColumnName("id_project");
            entity.Property(e => e.Name)
                .HasColumnType(SqlTypeVarchar)
                .HasColumnName("name");

            entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.Columns)
                .HasForeignKey(d => d.IdProject)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");

            entity.HasIndex(e => e.Id, "IX_Project_id").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Background)
                .HasColumnType(SqlTypeVarchar)
                .HasColumnName("background");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType(SqlTypeDateTime)
                .HasColumnName("creation_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Mute)
                .IsRequired()
                .HasDefaultValueSql("FALSE")
                .HasColumnType(SqlTypeBoolean)
                .HasColumnName("mute");
            entity.Property(e => e.Name)
                .HasColumnType(SqlTypeVarchar)
                .HasColumnName("name");
            entity.Property(e => e.Thumbnail)
                .HasColumnType(SqlTypeVarchar)
                .HasColumnName("thumbnail");
        });
    }
}

public static class DbLocation
{
    private const string AppFolderName = "ProjectPlan";
    private const string DbFileName = "projectplan.db";

    public static string DbDirectoryPath { get; } = Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
        AppFolderName);

    public static string DbFilePath { get; } = Path.Combine(DbDirectoryPath, DbFileName);

    public static string ConnectionString { get; } = $"Data Source={DbFilePath}";

    public static void EnsureDirectoryExists()
    {
        Directory.CreateDirectory(DbDirectoryPath);
    }
}
