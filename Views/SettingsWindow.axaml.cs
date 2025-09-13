// Views/SettingsWindow.axaml.cs
// 日本語概要: 参照ボタンでフォルダ/ファイルダイアログを開き、選択後にフォーカス検証コマンドを呼ぶ。
using Avalonia.Controls;
using Avalonia.Interactivity;
using AurigaFrontend.ViewModels;

namespace AurigaFrontend.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private async void BrowseAuriga_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel vm) return;
        var dlg = new OpenFolderDialog { Title = "Auriga があるフォルダを選択" };
        var dir = await dlg.ShowAsync(this);
        if (!string.IsNullOrWhiteSpace(dir))
        {
            vm.AurigaPath = dir;
            vm.LostFocusValidateAurigaCommand.Execute(null);
        }
    }

    private async void BrowseClient_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not SettingsViewModel vm) return;
        var dlg = new OpenFileDialog
        {
            Title = "Ragnarok Online クライアント EXE を選択",
            AllowMultiple = false
        };
        if (OperatingSystem.IsWindows())
            dlg.Filters!.Add(new FileDialogFilter { Name = "実行ファイル", Extensions = { "exe" } });

        var files = await dlg.ShowAsync(this);
        if (files is { Length: > 0 })
        {
            vm.ClientExePath = files[0];
            vm.LostFocusValidateClientCommand.Execute(null);
        }
    }
}
