// Constants.cs
// 日本語概要: アプリ全体で用いる定数を一元管理。フォルダ名や実行ファイル名をここで定義し、起動引数で上書き可能にします。
namespace AurigaFrontend;

public static class Constants
{
    // ログ・設定ファイル関連
    public const string AppName = "AurigaFrontend";
    public const string SettingsFileName = "settings.json";
    public const string LogsDirName = "logs";
    public const string LogFilePattern = "log-.txt";

    // サーバー実行ファイル名（Auriga の既定構成を想定）
    public const string LoginServerExe = "login-server";
    public const string CharServerExe  = "char-server";
    public const string MapServerExe   = "map-server";

    // Windows の場合 exe を付与するための補助
    public static string WithPlatformExt(string file)
    {
        // Windows 環境なら .exe を付ける（Auriga のビルド成果物に合わせて調整可能）
        if (OperatingSystem.IsWindows()) return file + ".exe";
        return file;
    }

    // UI 文言（必要に応じてここで一括管理）
    public const string StatusOk = "OK";
    public const string StatusWarning = "警告";
}
