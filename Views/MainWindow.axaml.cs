// C#
using Avalonia.Controls;
using AurigaPlus.ViewModels;

namespace AurigaPlus.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // DataContextが変更されたときにイベントハンドラを登録
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object? sender, System.EventArgs e)
        {
            // 古いViewModelのイベント購読を解除（もしあれば）
            if (DataContext is MainWindowViewModel oldVm)
            {
                oldVm.OpenSettingsRequested -= ShowSettingsDialog;
            }

            // 新しいViewModelのイベントを購読
            if (DataContext is MainWindowViewModel newVm)
            {
                newVm.OpenSettingsRequested += ShowSettingsDialog;
            }
        }

        /// <summary>
        /// 設定ダイアログを表示します。
        /// </summary>
        private async void ShowSettingsDialog(SettingsViewModel settingsVm)
        {
            var dialog = new SettingsWindow(settingsVm);
            await dialog.ShowDialog(this); // thisを渡してオーナーウィンドウを設定
        }
    }
}
