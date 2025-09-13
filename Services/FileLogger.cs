// C#
using System;
using System.IO;

namespace AurigaPlus.Services
{
    /// <summary>
    /// エラーログをファイルに記録する機能を提供します。
    /// </summary>
    public static class FileLogger
    {
        private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "error_log.txt");
        private static readonly object LockObj = new object();

        /// <summary>
        /// エラーログをファイルに追記します。
        /// </summary>
        /// <param name="message">記録するエラーメッセージ。</param>
        /// <param name="exception">発生した例外（オプション）。</param>
        public static void Log(string message, Exception? exception = null)
        {
            try
            {
                lock (LockObj)
                {
                    using (var writer = new StreamWriter(LogFilePath, true)) // trueで追記モード
                    {
                        writer.WriteLine("==================================================");
                        writer.WriteLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        writer.WriteLine($"Message: {message}");

                        if (exception != null)
                        {
                            writer.WriteLine("--- Exception Details ---");
                            writer.WriteLine($"Type: {exception.GetType().FullName}");
                            writer.WriteLine($"Exception Message: {exception.Message}");
                            writer.WriteLine($"Stack Trace: {exception.StackTrace}");
                            writer.WriteLine("---------------------------");
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                // ロガー自体が失敗した場合は、デバッグコンソールに出力するしかない
                System.Diagnostics.Debug.WriteLine($"FATAL: FileLogger failed. Original message: '{message}'. Error: {ex.Message}");
            }
        }
    }
}
