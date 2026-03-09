using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using ProjectPlan.ViewModels;

namespace ProjectPlan.Infrastructure;

public sealed class HasPreviousColumnConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return false;

        if (values[0] is not ProjectViewModel projectVm)
            return false;

        if (values[1] is not int columnId)
            return false;

        var index = IndexOfColumn(projectVm, columnId);
        return index > 0;
    }

    private static int IndexOfColumn(ProjectViewModel projectVm, int columnId)
    {
        for (var i = 0; i < projectVm.Columns.Count; i++)
        {
            if (projectVm.Columns[i].Id == columnId)
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

        if (values[0] is not ProjectViewModel projectVm)
            return false;

        if (values[1] is not int columnId)
            return false;

        var index = IndexOfColumn(projectVm, columnId);
        return index >= 0 && index < projectVm.Columns.Count - 1;
    }

    private static int IndexOfColumn(ProjectViewModel projectVm, int columnId)
    {
        for (var i = 0; i < projectVm.Columns.Count; i++)
        {
            if (projectVm.Columns[i].Id == columnId)
                return i;
        }

        return -1;
    }
}
