// C#
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AurigaPlus.ViewModels;
using AurigaPlus.Views;
using AurigaPlus.Models;

namespace AurigaPlus;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            var settings = AppSettings.Load();

            // 初回起動時や設定が不完全な場合は、設定ウィンドウを先に表示する
            if (string.IsNullOrWhiteSpace(settings.AurigaPath) || string.IsNullOrWhiteSpace(settings.ClientPath))
            {
                var settingsVm = new SettingsViewModel(settings);
                var settingsWindow = new SettingsWindow(settingsVm);
                
                // ViewModelのイベントに関数を登録
                settingsVm.SettingsSaved += ShowMainWindowAfterSettings;
                desktop.MainWindow = settingsWindow;
            }
            else
            {
                // 設定が存在する場合は、メインウィンドウを直接表示
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(settings)
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// 設定完了後にメインウィンドウを表示するためのコールバック。
    /// </summary>
    private void ShowMainWindowAfterSettings()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var settings = AppSettings.Load(); // 最新の設定を再読み込み
            var oldWindow = desktop.MainWindow;

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(settings)
            };
            
            if (desktop.MainWindow is not null)
            {
                desktop.MainWindow.Show();
            }
            oldWindow?.Close(); // 古い設定ウィンドウを閉じる
        }
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}