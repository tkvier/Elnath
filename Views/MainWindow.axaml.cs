// C#
using Avalonia.Controls;
using Elnath.ViewModels;
using System;

namespace Elnath.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm && vm.StopAllServersCommand.CanExecute(null))
            {
                // 現在のクロージングイベントをキャンセルします。
                e.Cancel = true;

                // Close()を再度呼び出したときに再入しないように、イベントから登録解除します。
                Closing -= MainWindow_Closing;

                // 非同期でサーバーを停止します。
                await vm.StopAllServersCommand.ExecuteAsync(null);

                // これで、ウィンドウを閉じます。これにより、イベントハンドラが再度トリガーされることはありません。
                Close();
            }
        }

        /// <summary>
        /// ログ表示用のTextBoxのテキストが変更されたときに、一番下までスクロールさせます。
        /// </summary>
        private void LogBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.CaretIndex = textBox.Text?.Length ?? 0;
            }
        }
    }
}