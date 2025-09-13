// App.xaml.cs
// 日本語概要: アプリの起動。初回は設定ウィンドウを表示し、保存後にメイン画面へ遷移。
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AurigaFrontend.ViewModels;
using AurigaFrontend.Views;
using AurigaFrontend.Services;

namespace AurigaFrontend;

public partial class App : Application
{
    public static Models.AppSettings Settings = Models.AppSettings.LoadOrDefault();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        LogService.Initialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainVm = new MainWindowViewModel(Settings);
            var main = new MainWindow { DataContext = mainVm };

            // 初回設定が必要なら SettingsWindow を表示
            if (string.IsNullOrWhiteSpace(Settings.AurigaPath) || string.IsNullOrWhiteSpace(Settings.ClientExePath))
            {
                var sw = new SettingsWindow();
                var svm = new SettingsViewModel(Settings);
                svm.CloseWindowRequested = () =>
                {
                    // 保存後に設定を再読込してメインへ
                    Settings = Models.AppSettings.LoadOrDefault();
                    mainVm.ApplyNewSettings(Settings);
                    sw.Close();
                    desktop.MainWindow = main;
                    main.Show();
                };
                sw.DataContext = svm;
                desktop.MainWindow = sw;
                sw.Show();
            }
            else
            {
                desktop.MainWindow = main;
                main.Show();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
