// C#
using Avalonia.Controls;

namespace Elnath.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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