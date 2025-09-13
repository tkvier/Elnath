// C#
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using AurigaPlus.Models;
using AurigaPlus.Services;
using Avalonia.Threading;

namespace AurigaPlus.ViewModels
{
    /// <summary>
    /// メインウィンドウのViewModel。
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly AppSettings _settings;
        private readonly ServerManager _loginServer;
        private readonly ServerManager _charServer;
        private readonly ServerManager _mapServer;

        // 各サーバーのコンソール出力を保持します。
        [ObservableProperty]
        private string? _loginServerOutput;
        [ObservableProperty]
        private string? _charServerOutput;
        [ObservableProperty]
        private string? _mapServerOutput;

        /// <summary>
        /// 設定ウィンドウを開くようViewに要求するイベント。
        /// </summary>
        public event Action<SettingsViewModel>? OpenSettingsRequested;

        // コマンドを定義します。
        public IAsyncRelayCommand RestartLoginServerCommand { get; }
        public IAsyncRelayCommand StopLoginServerCommand { get; }
        public IAsyncRelayCommand RestartCharServerCommand { get; }
        public IAsyncRelayCommand StopCharServerCommand { get; }
        public IAsyncRelayCommand RestartMapServerCommand { get; }
        public IAsyncRelayCommand StopMapServerCommand { get; }
        public IRelayCommand StartClientCommand { get; }
        public IRelayCommand OpenSettingsCommand { get; }

        public MainWindowViewModel(AppSettings settings)
        {
            _settings = settings;

            // サーバーマネージャーを初期化
            _loginServer = new ServerManager("ログインサーバー", "login-server", settings.AurigaPath ?? "");
            _charServer = new ServerManager("キャラクターサーバー", "char-server", settings.AurigaPath ?? "");
            _mapServer = new ServerManager("マップサーバー", "map-server", settings.AurigaPath ?? "");

            // サーバー出力をUIに反映するためのイベントハンドラを登録
            SetupServerOutputHandling();

            // コマンドを初期化
            RestartLoginServerCommand = new AsyncRelayCommand(_loginServer.RestartAsync);
            StopLoginServerCommand = new AsyncRelayCommand(_loginServer.StopAsync);
            RestartCharServerCommand = new AsyncRelayCommand(_charServer.RestartAsync);
            StopCharServerCommand = new AsyncRelayCommand(_charServer.StopAsync);
            RestartMapServerCommand = new AsyncRelayCommand(_mapServer.RestartAsync);
            StopMapServerCommand = new AsyncRelayCommand(_mapServer.StopAsync);
            StartClientCommand = new RelayCommand(StartClient);
            OpenSettingsCommand = new RelayCommand(OpenSettings);

            // アプリケーション起動時に全サーバーを起動
            StartAllServers();
        }

        private void SetupServerOutputHandling()
        {
            var loginSb = new StringBuilder();
            _loginServer.OutputDataReceived += (line) => AppendOutput(loginSb, (output) => LoginServerOutput = output, line);

            var charSb = new StringBuilder();
            _charServer.OutputDataReceived += (line) => AppendOutput(charSb, (output) => CharServerOutput = output, line);

            var mapSb = new StringBuilder();
            _mapServer.OutputDataReceived += (line) => AppendOutput(mapSb, (output) => MapServerOutput = output, line);
        }

        private void AppendOutput(StringBuilder sb, Action<string> setOutput, string line)
        {
            // UIスレッドでテキストを更新
            Dispatcher.UIThread.Post(() =>
            {
                sb.AppendLine(line);
                // パフォーマンスのため、一定量を超えたら古いログを削除
                if (sb.Length > 20000)
                {
                    sb.Remove(0, sb.Length - 15000);
                }
                setOutput(sb.ToString());
            });
        }

        private void StartAllServers()
        {
            if (string.IsNullOrWhiteSpace(_settings.AurigaPath))
            {
                LoginServerOutput = CharServerOutput = MapServerOutput = "Aurigaのパスが設定されていません。設定画面からパスを指定してください。";
                return;
            }
            _loginServer.StartAsync();
            _charServer.StartAsync();
            _mapServer.StartAsync();
        }

        private void StartClient()
        {
            if (string.IsNullOrWhiteSpace(_settings.ClientPath) || !System.IO.File.Exists(_settings.ClientPath))
            {
                FileLogger.Log("クライアントのパスが正しく設定されていません。メイン画面にエラーを表示する機能は未実装です。");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(_settings.ClientPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                FileLogger.Log("クライアントの起動に失敗しました。", ex);
            }
        }

        private void OpenSettings()
        {
            var settingsVm = new SettingsViewModel(_settings);
            OpenSettingsRequested?.Invoke(settingsVm);
        }

        public void Dispose()
        {
            _loginServer.Dispose();
            _charServer.Dispose();
            _mapServer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
