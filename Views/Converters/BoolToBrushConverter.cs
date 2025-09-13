// Views/Converters/BoolToBrushConverter.cs
// 日本語概要: ステータス表示でエラー時は赤、通常時はアクセント色にするためのコンバータ。
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace AurigaFrontend.Views.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public IBrush ErrorBrush { get; set; } = Brushes.Red;
    public IBrush OkBrush { get; set; } = Brushes.ForestGreen;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b) return b ? ErrorBrush : OkBrush;
        return OkBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}
