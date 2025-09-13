// ViewModels/MainWindowViewModel.cs
// 日本語概要: メイン画面のロジック。3サーバーの起動/再起動/終了、出力のバインディング、クライアント起動を実装します。
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AurigaFrontend.Services;

namespace AurigaFrontend.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly Models.AppSettings _settings;
    private ServerProcessManager? _manager;

    [ObservableProperty] private bool loginRunning;
    [ObservableProperty] private bool charRunning;
    [ObservableProperty] private bool mapRunning;

    public ObservableCollection<string> LoginLog { get; } = new();
    public ObservableCollection<string> CharLog  { get; } = new();
    public ObservableCollection<string> MapLog   { get; } = new();

    [ObservableProperty] private string statusBarText = "準備完了";

    public MainWindowViewModel(Models.AppSettings settings)
    {
        _settings = settings;
        if (!string.IsNullOrEmpty(_settings.AurigaPath))
        {
            _manager = new ServerProcessManager(_settings.AurigaPath);
            _manager.OutputReceived += OnOutputReceived;
        }
        else
        {
            StatusBarText = "設定が未完了です。パスを設定してください。";
        }
    }

    private void EnsureManager()
    {
        if (_manager == null && !string.IsNullOrEmpty(_settings.AurigaPath))
        {
            _manager = new ServerProcessManager(_settings.AurigaPath);
            _manager.OutputReceived += OnOutputReceived;
        }
    }

    private void OnOutputReceived(object? sender, ProcessOutputEventArgs e)
    {
        // UI スレッドに切り替え
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var line = (e.IsError ? "[ERR] " : "") + e.Line;
            switch (e.Kind)
            {
                case ServerKind.Login: LoginLog.Add(line); LoginRunning = _manager!.IsRunning(ServerKind.Login); break;
                case ServerKind.Char:  CharLog.Add(line);  CharRunning  = _manager!.IsRunning(ServerKind.Char);  break;
                case ServerKind.Map:   MapLog.Add(line);   MapRunning   = _manager!.IsRunning(ServerKind.Map);   break;
            }
        });
    }

    [RelayCommand]
    public void OpenSettings()
    {
        // メインウィンドウ側でハンドリング（View からダイアログ表示）
    }

    [RelayCommand]
    public void StartLogin()
    {
        EnsureManager();
        if (_manager == null) return; // 条件スキップ
        if (_manager.Start(ServerKind.Login))
            StatusBarText = "Login Server を起動しました";
    }

    [RelayCommand]
    public void StopLogin()
    {
        if (_manager == null) return; // 条件スキップ
        if (_manager.Stop(ServerKind.Login))
            StatusBarText = "Login Server を停止しました";
    }

    [RelayCommand]
    public void RestartLogin()
    {
        if (_manager == null) return; // 条件スキップ
        if (_manager.Restart(ServerKind.Login))
            StatusBarText = "Login Server を再起動しました";
    }

    [RelayCommand] public void StartChar()    { EnsureManager(); if (_manager?.Start(ServerKind.Char) == true)  StatusBarText = "Char Server を起動しました"; }
    [RelayCommand] public void StopChar()     { if (_manager?.Stop(ServerKind.Char) == true)  StatusBarText = "Char Server を停止しました"; }
    [RelayCommand] public void RestartChar()  { if (_manager?.Restart(ServerKind.Char) == true) StatusBarText = "Char Server を再起動しました"; }
    [RelayCommand] public void StartMap()     { EnsureManager(); if (_manager?.Start(ServerKind.Map) == true)   StatusBarText = "Map Server を起動しました"; }
    [RelayCommand] public void StopMap()      { if (_manager?.Stop(ServerKind.Map) == true)   StatusBarText = "Map Server を停止しました"; }
    [RelayCommand] public void RestartMap()   { if (_manager?.Restart(ServerKind.Map) == true) StatusBarText = "Map Server を再起動しました"; }

    [RelayCommand]
    public void LaunchClient()
    {
        try
        {
            var exe = _settings.ClientExePath;
            if (string.IsNullOrWhiteSpace(exe) || !File.Exists(exe)) return; // 条件スキップ

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = exe,
                WorkingDirectory = Path.GetDirectoryName(exe) ?? Environment.CurrentDirectory,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
            StatusBarText = "クライアントを起動しました（監視はしません）";
        }
        catch (Exception ex)
        {
            Services.LogService.Error("Client", "クライアント起動で例外が発生しました", ex);
            StatusBarText = "クライアント起動に失敗しました。ログを確認してください。";
        }
    }

    public void ApplyNewSettings(Models.AppSettings s)
    {
        // 設定変更適用（再生成）
        _manager = new ServerProcessManager(s.AurigaPath);
        _manager.OutputReceived += OnOutputReceived;
        StatusBarText = "設定を適用しました";
    }
}
