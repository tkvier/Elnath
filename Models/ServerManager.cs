// C#
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Elnath.Services;

namespace Elnath.Models
{
    /// <summary>
    /// サーバープロセスの状態を表す列挙型。
    /// </summary>
    public enum ServerStatus
    {
        Stopped,
        Running,
        Error
    }

    /// <summary>
    /// サーバープロセスを管理するクラス。
    /// </summary>
    public class ServerManager : IDisposable
    {
        private readonly string _serverName;
        private readonly string _aurigaPath;
        private readonly string _executableName;
        private Process? _process;

        /// <summary>
        /// サーバーの表示名。
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// サーバーの現在の状態。
        /// </summary>
        public ServerStatus Status { get; private set; } = ServerStatus.Stopped;

        /// <summary>
        /// プロセスの標準出力を受け取ったときに発生するイベント。
        /// </summary>
        public event Action<string>? OutputDataReceived;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="displayName">サーバーの表示名。</param>
        /// <param name="serverName">サーバーの内部名（実行ファイル名から拡張子を除いたもの）。</param>
        /// <param name="aurigaPath">Aurigaのルートパス。</param>
        public ServerManager(string displayName, string serverName, string aurigaPath)
        {
            DisplayName = displayName;
            _serverName = serverName;
            _aurigaPath = aurigaPath;
            _executableName = $"{serverName}.exe";
        }

        /// <summary>
        /// サーバーを非同期で起動します。
        /// </summary>
        public Task StartAsync()
        {
            return Task.Run(() =>
            {
                string exePath = Path.Combine(_aurigaPath, _executableName);
                if (!File.Exists(exePath))
                {
                    Status = ServerStatus.Error;
                    OutputDataReceived?.Invoke($"エラー: 実行ファイルが見つかりません: {exePath}");
                    FileLogger.Log($"{DisplayName}の起動失敗: 実行ファイルが見つかりません。", new FileNotFoundException(exePath));
                    return;
                }

                try
                {
                    _process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = exePath,
                            WorkingDirectory = _aurigaPath,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                        },
                        EnableRaisingEvents = true
                    };

                    _process.OutputDataReceived += (sender, args) => { if (args.Data != null) OutputDataReceived?.Invoke(args.Data); };
                    _process.ErrorDataReceived += (sender, args) => { if (args.Data != null) OutputDataReceived?.Invoke($"[ERROR] {args.Data}"); };
                    _process.Exited += (sender, args) =>
                    {
                        Status = ServerStatus.Stopped;
                        OutputDataReceived?.Invoke("サーバーが停止しました。");
                    };

                    _process.Start();
                    _process.BeginOutputReadLine();
                    _process.BeginErrorReadLine();

                    Status = ServerStatus.Running;
                    OutputDataReceived?.Invoke("サーバーを起動しました。");
                }
                catch (Exception ex)
                {
                    Status = ServerStatus.Error;
                    var errorMessage = $"サーバーの起動中に予期せぬエラーが発生しました: {ex.Message}";
                    OutputDataReceived?.Invoke(errorMessage);
                    FileLogger.Log($"{DisplayName}の起動に失敗しました。", ex);
                }
            });
        }

        /// <summary>
        /// サーバーを非同期で停止します。
        /// </summary>
        public Task StopAsync()
        {
            return Task.Run(() =>
            {
                if (_process == null || _process.HasExited)
                {
                    return; // すでに停止している
                }

                try
                {
                    if (!_process.HasExited)
                    {
                        _process.Kill();
                        // 5秒待ってプロセスが終了したか確認
                        if (!_process.WaitForExit(5000))
                        {
                            var message = $"[ERROR] プロセスの終了に失敗しました: {_process.ProcessName} (PID: {_process.Id})";
                            OutputDataReceived?.Invoke(message);
                            FileLogger.Log($"{DisplayName}の停止失敗: {message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"サーバーの停止中にエラーが発生しました: {ex.Message}";
                    OutputDataReceived?.Invoke(errorMessage);
                    FileLogger.Log($"{DisplayName}の停止に失敗しました。", ex);
                }
                finally
                {
                    _process?.Dispose();
                    _process = null;
                    Status = ServerStatus.Stopped;
                }
            });
        }

        /// <summary>
        /// サーバーを非同期で再起動します。
        /// </summary>
        public async Task RestartAsync()
        {
            await StopAsync();
            // 少し待機してから再起動
            await Task.Delay(500);
            await StartAsync();
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            StopAsync().Wait(); // 同期的に待機
            GC.SuppressFinalize(this);
        }
    }
}
