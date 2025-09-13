// C#
using System.Text.Json;
using System.IO;
using AurigaPlus.Services;

namespace AurigaPlus.Models
{
    /// <summary>
    /// アプリケーションの設定を管理するクラス。
    /// AurigaのパスとRagnarok Onlineクライアントのパスを保持します。
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Aurigaサーバー群が格納されているディレクトリのパス。
        /// </summary>
        public string? AurigaPath { get; set; }

        /// <summary>
        /// Ragnarok Onlineクライアント実行ファイルのパス。
        /// </summary>
        public string? ClientPath { get; set; }

        private static string GetSettingsFilePath()
        {
            // アプリケーションの実行ファイルがあるディレクトリに設定ファイルを保存します。
            string exePath = System.AppContext.BaseDirectory;
            return Path.Combine(exePath, "settings.json");
        }

        /// <summary>
        /// 現在の設定をJSONファイルに保存します。
        /// </summary>
        public void Save()
        {
            try
            {
                string filePath = GetSettingsFilePath();
                string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);
            }
            catch (System.Exception ex)
            {
                FileLogger.Log("設定ファイルの保存に失敗しました。", ex);
            }
        }

        /// <summary>
        /// JSONファイルから設定を読み込みます。
        /// ファイルが存在しない場合は、新しいインスタンスを返します。
        /// </summary>
        /// <returns>読み込まれた設定情報。ファイルが存在しない場合はデフォルト設定。</returns>
        public static AppSettings Load()
        {
            try
            {
                string filePath = GetSettingsFilePath();
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                    return settings ?? new AppSettings();
                }
            }
            catch (System.Exception ex)
            {
                FileLogger.Log("設定ファイルの読み込みに失敗しました。", ex);
            }
            
            return new AppSettings();
        }
    }
}
