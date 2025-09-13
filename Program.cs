// Program.cs
// 日本語概要: 起動引数で Auriga パス/クライアントパスを上書き可能に。未設定時は設定ウィンドウが開きます。
using Avalonia;
using System.Linq;

namespace AurigaFrontend;

internal static class Program
{
    public static void Main(string[] args)
    {
        // 設定の読み込みと上書き
        var s = Models.AppSettings.LoadOrDefault();

        foreach (var a in args)
        {
            if (a.StartsWith("--auriga-path=", StringComparison.OrdinalIgnoreCase))
                s.AurigaPath = a.Split('=', 2)[1].Trim('"');
            if (a.StartsWith("--client-path=", StringComparison.OrdinalIgnoreCase))
                s.ClientExePath = a.Split('=', 2)[1].Trim('"');
        }

        // 上書きがあれば保存（有効性チェックは SettingsWindow に委譲）
        try { s.Save(); } catch (Exception ex) { Services.LogService.Error("Boot", "起動時設定保存で例外", ex); }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
