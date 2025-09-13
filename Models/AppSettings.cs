// Models/AppSettings.cs
// 日本語概要: ユーザー設定（Auriga のパス、クライアントパス）を JSON で保存・読み込みします。アプリの AppData 配下に格納します。
using System.Text.Json;

namespace AurigaFrontend.Models;

public class AppSettings
{
    public string AurigaPath { get; set; } = string.Empty;
    public string ClientExePath { get; set; } = string.Empty;

    public static string GetAppDataDir()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AurigaFrontend.Constants.AppName);
        Directory.CreateDirectory(dir);
        return dir;
    }

    public static string GetSettingsPath()
        => Path.Combine(GetAppDataDir(), AurigaFrontend.Constants.SettingsFileName);

    public static AppSettings LoadOrDefault()
    {
        var path = GetSettingsPath();
        if (!File.Exists(path)) return new AppSettings();
        try
        {
            var json = File.ReadAllText(path);
            var obj = JsonSerializer.Deserialize<AppSettings>(json);
            return obj ?? new AppSettings();
        }
        catch
        {
            // 読み込み失敗時はデフォルトを返し、詳細はロガー側で記録
            return new AppSettings();
        }
    }

    public void Save()
    {
        var path = GetSettingsPath();
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}
