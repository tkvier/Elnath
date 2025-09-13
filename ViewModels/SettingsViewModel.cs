// C#
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using AurigaPlus.Models;
using System;

namespace AurigaPlus.ViewModels
{
    /// <summary>
    /// 設定ウィンドウのViewModel。
    /// </summary>
    public partial class SettingsViewModel : ViewModelBase
    {
        // ObservableProperty属性により、プロパティの変更通知が自動的に実装されます。
        [ObservableProperty]
        private string? _aurigaPath;

        [ObservableProperty]
        private string? _clientPath;

        [ObservableProperty]
        private string? _statusMessage;

        [ObservableProperty]
        private bool _isSaveEnabled;

        public AppSettings Settings { get; }

        /// <summary>
        /// 設定が保存されたときに発生するイベント。
        /// </summary>
        public event Action? SettingsSaved;

        // IRelayCommandは、ViewからのイベントをViewModelで処理するためのコマンドです。
        public IRelayCommand ValidateAurigaPathCommand { get; }
        public IRelayCommand ValidateClientPathCommand { get; }
        public IRelayCommand SaveCommand { get; }

        public SettingsViewModel(AppSettings settings)
        {
            Settings = settings;
            _aurigaPath = settings.AurigaPath;
            _clientPath = settings.ClientPath;

            ValidateAurigaPathCommand = new RelayCommand(ValidateAurigaPath);
            ValidateClientPathCommand = new RelayCommand(ValidateClientPath);
            SaveCommand = new RelayCommand(SaveSettings);

            // 初期状態のバリデーションを実行
            ValidatePaths();
        }

        /// <summary>
        /// Aurigaのパスが正しいか検証します。
        /// </summary>
        private void ValidateAurigaPath()
        {
            if (string.IsNullOrWhiteSpace(AurigaPath) || !Directory.Exists(AurigaPath))
            {
                StatusMessage = "Aurigaのパスが存在しません。";
                IsSaveEnabled = false;
                return;
            }

            // 必須のサーバー実行ファイルが存在するかチェック
            bool loginServerOk = File.Exists(Path.Combine(AurigaPath, "login-server.exe"));
            bool charServerOk = File.Exists(Path.Combine(AurigaPath, "char-server.exe"));
            bool mapServerOk = File.Exists(Path.Combine(AurigaPath, "map-server.exe"));

            if (!loginServerOk || !charServerOk || !mapServerOk)
            {
                StatusMessage = "サーバー実行ファイル (login/char/map-server.exe) が見つかりません。";
                IsSaveEnabled = false;
                return;
            }

            // 全てのチェックが通ったら、もう片方のパスも検証する
            ValidatePaths();
        }

        /// <summary>
        /// Ragnarok Onlineクライアントのパスが正しいか検証します。
        /// </summary>
        private void ValidateClientPath()
        {
            if (string.IsNullOrWhiteSpace(ClientPath) || !File.Exists(ClientPath))
            {
                StatusMessage = "クライアント実行ファイルが存在しません。";
                IsSaveEnabled = false;
                return;
            }

            if (!Path.GetExtension(ClientPath).Equals(".exe", System.StringComparison.OrdinalIgnoreCase))
            {
                StatusMessage = "クライアントパスは実行ファイル (.exe) である必要があります。";
                IsSaveEnabled = false;
                return;
            }
            
            // 全てのチェックが通ったら、もう片方のパスも検証する
            ValidatePaths();
        }

        /// <summary>
        /// 両方のパスを検証し、問題がなければ保存ボタンを有効にします。
        /// </summary>
        private void ValidatePaths()
        {
            // Aurigaパスの簡易チェック
            if (string.IsNullOrWhiteSpace(AurigaPath) || !Directory.Exists(AurigaPath) ||
                !File.Exists(Path.Combine(AurigaPath, "login-server.exe")) ||
                !File.Exists(Path.Combine(AurigaPath, "char-server.exe")) ||
                !File.Exists(Path.Combine(AurigaPath, "map-server.exe")))
            {
                IsSaveEnabled = false;
                StatusMessage = "Aurigaのパスが正しくありません。";
                return;
            }

            // クライアントパスの簡易チェック
            if (string.IsNullOrWhiteSpace(ClientPath) || !File.Exists(ClientPath) || !Path.GetExtension(ClientPath).Equals(".exe", System.StringComparison.OrdinalIgnoreCase))
            {
                IsSaveEnabled = false;
                StatusMessage = "クライアントのパスが正しくありません。";
                return;
            }

            // すべての検証を通過
            StatusMessage = "パスは正常です。設定を保存できます。";
            IsSaveEnabled = true;
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        private void SaveSettings()
        {
            if (!IsSaveEnabled) return;

            Settings.AurigaPath = AurigaPath;
            Settings.ClientPath = ClientPath;
            Settings.Save();
            StatusMessage = "設定を保存しました。";

            // Viewに保存が完了したことを通知
            SettingsSaved?.Invoke();
        }
    }
}
