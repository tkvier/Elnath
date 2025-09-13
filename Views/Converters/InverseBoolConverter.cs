// Views/Converters/InverseBoolConverter.cs
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AurigaFrontend.Views.Converters;

public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? !b : true;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

// Views/Converters/RunningToTextConverter.cs
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AurigaFrontend.Views.Converters;

public class RunningToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b ? (b ? "稼働中" : "停止") : "不明";

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
