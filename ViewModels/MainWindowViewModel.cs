// C#
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Elnath.Models;
using Elnath.Services;
using Avalonia.Media;
using Avalonia.Threading;

namespace Elnath.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly AppSettings _settings;
    private ServerManager? _loginServer, _charServer, _mapServer;

    private static readonly IBrush RedBrush = Brushes.Red;
    private static readonly IBrush WhiteBrush = Brushes.White;

    // --- Path and UI State Properties ---
    [ObservableProperty]
    private string? _aurigaPath;

    [ObservableProperty]
    private string? _clientPath;

    [ObservableProperty] private IBrush? _aurigaPathBrush;
    [ObservableProperty] private IBrush? _clientPathBrush;
    [ObservableProperty] private bool _isAurigaPathReadOnly;
    [ObservableProperty] private bool _isServerStartEnabled;
    [ObservableProperty] private bool _isServerStopEnabled;
    [ObservableProperty] private bool _isClientStartEnabled;
    [ObservableProperty] private bool _isClientSettingsEnabled;
    [ObservableProperty] private bool _isPerServerControlEnabled;

    // --- Server Log Properties ---
    [ObservableProperty] private string? _loginServerOutput;
    [ObservableProperty] private string? _charServerOutput;
    [ObservableProperty] private string? _mapServerOutput;

    // --- Commands ---
    public IAsyncRelayCommand StartAllServersCommand { get; }
    public IAsyncRelayCommand StopAllServersCommand { get; }
    public IRelayCommand StartClientCommand { get; }
    public IRelayCommand StartClientSetupCommand { get; }
    public IAsyncRelayCommand RestartLoginServerCommand { get; }
    public IAsyncRelayCommand StopLoginServerCommand { get; }
    public IAsyncRelayCommand RestartCharServerCommand { get; }
    public IAsyncRelayCommand StopCharServerCommand { get; }
    public IAsyncRelayCommand RestartMapServerCommand { get; }
    public IAsyncRelayCommand StopMapServerCommand { get; }

    public MainWindowViewModel(AppSettings settings)
    {
        _settings = settings;
        _aurigaPath = settings.AurigaPath;
        _clientPath = settings.ClientPath;

        // コマンドを初期化
        StartAllServersCommand = new AsyncRelayCommand(StartAllServers, () => IsServerStartEnabled);
        StopAllServersCommand = new AsyncRelayCommand(StopAllServers, () => IsServerStopEnabled);
        StartClientCommand = new RelayCommand(StartClient, () => IsClientStartEnabled);
        StartClientSetupCommand = new RelayCommand(StartClientSetup, () => IsClientSettingsEnabled);
        
        // 個別サーバーのコマンド (有効/無効は IsPerServerControlEnabled で一括管理)
        RestartLoginServerCommand = new AsyncRelayCommand(async () => await _loginServer?.RestartAsync()!, () => IsPerServerControlEnabled);
        StopLoginServerCommand = new AsyncRelayCommand(async () => await _loginServer?.StopAsync()!, () => IsPerServerControlEnabled);
        RestartCharServerCommand = new AsyncRelayCommand(async () => await _charServer?.RestartAsync()!, () => IsPerServerControlEnabled);
        StopCharServerCommand = new AsyncRelayCommand(async () => await _charServer?.StopAsync()!, () => IsPerServerControlEnabled);
        RestartMapServerCommand = new AsyncRelayCommand(async () => await _mapServer?.RestartAsync()!, () => IsPerServerControlEnabled);
        StopMapServerCommand = new AsyncRelayCommand(async () => await _mapServer?.StopAsync()!, () => IsPerServerControlEnabled);

        // 初期検証
        ValidateAurigaPath();
        ValidateClientPath();
    }

    // --- Validation Logic ---
    partial void OnAurigaPathChanged(string? value) => ValidateAurigaPath();
    partial void OnClientPathChanged(string? value) => ValidateClientPath();

    private void ValidateAurigaPath()
    {
        if (!string.IsNullOrWhiteSpace(AurigaPath) && Directory.Exists(AurigaPath) &&
            File.Exists(Path.Combine(AurigaPath, "login-server.exe")) &&
            File.Exists(Path.Combine(AurigaPath, "char-server.exe")) &&
            File.Exists(Path.Combine(AurigaPath, "map-server.exe")))
        {
            AurigaPathBrush = WhiteBrush;
            IsServerStartEnabled = true;
            _settings.AurigaPath = AurigaPath;
            _settings.Save();
        }
        else
        {
            AurigaPathBrush = RedBrush;
            IsServerStartEnabled = false;
        }
        StartAllServersCommand.NotifyCanExecuteChanged();
    }

    private void ValidateClientPath()
    {
        if (!string.IsNullOrWhiteSpace(ClientPath) && File.Exists(ClientPath) &&
            Path.GetExtension(ClientPath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
        {
            ClientPathBrush = WhiteBrush;
            IsClientStartEnabled = true;
            IsClientSettingsEnabled = File.Exists(Path.Combine(Path.GetDirectoryName(ClientPath) ?? "", "Setup.exe"));
            _settings.ClientPath = ClientPath;
            _settings.Save();
        }
        else
        {
            ClientPathBrush = RedBrush;
            IsClientStartEnabled = false;
            IsClientSettingsEnabled = false;
        }
        StartClientCommand.NotifyCanExecuteChanged();
        StartClientSetupCommand.NotifyCanExecuteChanged();
    }

    // --- Server Control Logic ---
    private async Task StartAllServers()
    {
        if (!IsServerStartEnabled) return;

        IsAurigaPathReadOnly = true;
        IsServerStartEnabled = false;
        IsServerStopEnabled = true;
        IsPerServerControlEnabled = true;
        NotifyAllCanExecuteChanged();

        // サーバーマネージャーを初期化・起動
        _loginServer = new ServerManager("ログインサーバー", "login-server", AurigaPath!);
        _charServer = new ServerManager("キャラクターサーバー", "char-server", AurigaPath!);
        _mapServer = new ServerManager("マップサーバー", "map-server", AurigaPath!);
        SetupServerOutputHandling();

        await Task.WhenAll(
            _loginServer.StartAsync(),
            _charServer.StartAsync(),
            _mapServer.StartAsync()
        );
    }

    private async Task StopAllServers()
    {
        await Task.WhenAll(
            _loginServer?.StopAsync() ?? Task.CompletedTask,
            _charServer?.StopAsync() ?? Task.CompletedTask,
            _mapServer?.StopAsync() ?? Task.CompletedTask
        );

        IsAurigaPathReadOnly = false;
        IsServerStartEnabled = true;
        IsServerStopEnabled = false;
        IsPerServerControlEnabled = false;
        NotifyAllCanExecuteChanged();
    }

    // --- Client Control Logic ---
    private void StartClient() => ProcessRunning(ClientPath);
    private void StartClientSetup()
    {
        string? clientDir = Path.GetDirectoryName(ClientPath);
        if (clientDir != null) ProcessRunning(Path.Combine(clientDir, "Setup.exe"));
    }

    private void ProcessRunning(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName)) return;
        try
        {
            var processStartInfo = new ProcessStartInfo(fileName)
            {
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(fileName)
            };
            Process.Start(processStartInfo);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"{fileName} の起動に失敗しました。", ex);
        }
    }

    // --- Utility Methods ---
    private void SetupServerOutputHandling()
    {
        var loginSb = new StringBuilder();
        if (_loginServer != null) _loginServer.OutputDataReceived += (line) => AppendOutput(loginSb, (output) => LoginServerOutput = output, line);

        var charSb = new StringBuilder();
        if (_charServer != null) _charServer.OutputDataReceived += (line) => AppendOutput(charSb, (output) => CharServerOutput = output, line);

        var mapSb = new StringBuilder();
        if (_mapServer != null) _mapServer.OutputDataReceived += (line) => AppendOutput(mapSb, (output) => MapServerOutput = output, line);
    }

    private void AppendOutput(StringBuilder sb, Action<string> setOutput, string line)
    {
        Dispatcher.UIThread.Post(() =>
        {
            sb.AppendLine(line);
            if (sb.Length > 20000) sb.Remove(0, sb.Length - 15000);
            setOutput(sb.ToString());
        });
    }

    private void NotifyAllCanExecuteChanged()
    {
        StartAllServersCommand.NotifyCanExecuteChanged();
        StopAllServersCommand.NotifyCanExecuteChanged();
        RestartLoginServerCommand.NotifyCanExecuteChanged();
        StopLoginServerCommand.NotifyCanExecuteChanged();
        RestartCharServerCommand.NotifyCanExecuteChanged();
        StopCharServerCommand.NotifyCanExecuteChanged();
        RestartMapServerCommand.NotifyCanExecuteChanged();
        StopMapServerCommand.NotifyCanExecuteChanged();
    }

    public void Dispose()
    {
        // The closing event in MainWindow.axaml.cs now handles stopping servers.
        // This method is kept to fulfill the IDisposable interface.
        GC.SuppressFinalize(this);
    }
}