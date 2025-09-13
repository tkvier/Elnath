// C#
using Avalonia.Controls;
using Avalonia.Interactivity;
using AurigaPlus.ViewModels;

namespace AurigaPlus.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // ViewModelで設定が保存されたら、ウィンドウを閉じる
            viewModel.SettingsSaved += () => Close();
        }

        /// <summary>
        /// キャンセルボタンがクリックされたときの処理。
        /// </summary>
        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}