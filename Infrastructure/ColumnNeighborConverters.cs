using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Infrastructure;

public sealed class HasPreviousColumnConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return false;

        if (!TryGetColumns(values, out var columns))
            return false;

        if (!TryGetColumnId(values, out var columnId))
            return false;

        var index = IndexOfColumn(columns, columnId);
        return index > 0;
    }

    private static bool TryGetColumns(IList<object?> values, out IReadOnlyList<BoardColumnViewModel> columns)
    {
        columns = Array.Empty<BoardColumnViewModel>();

        // Pattern A: (columns, columnId)
        // Pattern B: (columns, columnsCount, columnId) -> columnsCount is used only to trigger re-evaluation on collection changes.
        if (values[0] is not IEnumerable<BoardColumnViewModel> enumerable)
            return false;

        columns = enumerable as IReadOnlyList<BoardColumnViewModel> ?? enumerable.ToList();
        return true;
    }

    private static bool TryGetColumnId(IList<object?> values, out int columnId)
    {
        columnId = default;
        var last = values[^1];
        return last is int id && (columnId = id) >= 0;
    }

    private static int IndexOfColumn(IReadOnlyList<BoardColumnViewModel> columns, int columnId)
    {
        for (var i = 0; i < columns.Count; i++)
        {
            if (columns[i].Id == columnId)
                return i;
        }

        return -1;
    }
}

public sealed class HasNextColumnConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return false;

        if (!TryGetColumns(values, out var columns))
            return false;

        if (!TryGetColumnId(values, out var columnId))
            return false;

        var index = IndexOfColumn(columns, columnId);
        return index >= 0 && index < columns.Count - 1;
    }

    private static bool TryGetColumns(IList<object?> values, out IReadOnlyList<BoardColumnViewModel> columns)
    {
        columns = Array.Empty<BoardColumnViewModel>();

        if (values[0] is not IEnumerable<BoardColumnViewModel> enumerable)
            return false;

        columns = enumerable as IReadOnlyList<BoardColumnViewModel> ?? enumerable.ToList();
        return true;
    }

    private static bool TryGetColumnId(IList<object?> values, out int columnId)
    {
        columnId = default;
        var last = values[^1];
        return last is int id && (columnId = id) >= 0;
    }

    private static int IndexOfColumn(IReadOnlyList<BoardColumnViewModel> columns, int columnId)
    {
        for (var i = 0; i < columns.Count; i++)
        {
            if (columns[i].Id == columnId)
                return i;
        }

        return -1;
    }
}
