// Services/LogService.cs
// 日本語概要: Serilog を用いて日次ロールのファイル出力。エラーは日時・対象・原因を明確に追記。
// 条件スキップはここでログしない設計にします（呼び出し側で記録しない運用）。
using Serilog;

namespace AurigaFrontend.Services;

public static class LogService
{
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;

        var appDir = Models.AppSettings.GetAppDataDir();
        var logsDir = Path.Combine(appDir, Constants.LogsDirName);
        Directory.CreateDirectory(logsDir);
        var logPath = Path.Combine(logsDir, Constants.LogFilePattern);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                shared: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        _initialized = true;
        Log.Information("ログ初期化完了");
    }

    public static void Error(string target, string message, Exception? ex = null)
    {
        // 失敗時の対象・原因・日時を明確に記録（Serilog が日時付与）
        if (ex != null)
            Serilog.Log.Error(ex, "[{Target}] {Message}", target, message);
        else
            Serilog.Log.Error("[{Target}] {Message}", target, message);
    }

    public static void Info(string target, string message)
    {
        Serilog.Log.Information("[{Target}] {Message}", target, message);
    }

    public static void Warn(string target, string message)
    {
        Serilog.Log.Warning("[{Target}] {Message}", target, message);
    }
}
