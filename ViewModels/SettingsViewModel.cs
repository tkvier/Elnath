// ViewModels/SettingsViewModel.cs
// 日本語概要: 初回設定ウィンドウのロジック。フォーカスが外れた時点とプロパティ更新でパス検証し、合格時のみ保存を有効化。
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace AurigaFrontend.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string aurigaPath = string.Empty;

    [ObservableProperty]
    private string clientExePath = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private bool statusIsError;

    [ObservableProperty]
    private bool canSave;

    public Action? CloseWindowRequested;

    public SettingsViewModel(Models.AppSettings settings)
    {
        AurigaPath = settings.AurigaPath;
        ClientExePath = settings.ClientExePath;
        ValidateAll();
    }

    [RelayCommand]
    public void LostFocusValidateAuriga() => ValidateAuriga();

    [RelayCommand]
    public void LostFocusValidateClient() => ValidateClient();

    private void ValidateAuriga()
    {
        // Auriga のパスが存在し、サーバー実行ファイルが揃っているかを確認
        if (string.IsNullOrWhiteSpace(AurigaPath) || !Directory.Exists(AurigaPath))
        {
            SetStatus("Auriga のパスが存在しません", error: true);
            CanSave = false;
            return;
        }

        var required = new[]
        {
            Constants.WithPlatformExt(Constants.LoginServerExe),
            Constants.WithPlatformExt(Constants.CharServerExe),
            Constants.WithPlatformExt(Constants.MapServerExe),
        };

        var missing = required.Where(n => !File.Exists(Path.Combine(AurigaPath, n))).ToList();
        if (missing.Any())
        {
            SetStatus($"Auriga の必須実行ファイルが見つかりません: {string.Join(", ", missing)}", error: true);
            CanSave = false;
        }
        else
        {
            SetStatus("Auriga パスは有効です", error: false);
            CanSave = IsBothValid();
        }
    }

    private void ValidateClient()
    {
        if (string.IsNullOrWhiteSpace(ClientExePath) || !File.Exists(ClientExePath))
        {
            SetStatus("Ragnarok Online クライアントの EXE が見つかりません", error: true);
            CanSave = false;
        }
        else
        {
            SetStatus("クライアントパスは有効です", error: false);
            CanSave = IsBothValid();
        }
    }

    private void ValidateAll()
    {
        // 初期表示時の総合チェック
        var oldStatus = StatusMessage;
        var oldErr = StatusIsError;
        ValidateAuriga();
        var okA = !StatusIsError;
        ValidateClient();
        var okB = !StatusIsError;
        if (okA && okB)
        {
            SetStatus("両方のパスが有効です", error: false);
            CanSave = true;
        }
        else
        {
            // 個別検証の結果を保つ
            StatusMessage = oldStatus;
            StatusIsError = oldErr;
            CanSave = false;
        }
    }

    private bool IsBothValid()
        => Directory.Exists(AurigaPath)
           && File.Exists(Path.Combine(AurigaPath, Constants.WithPlatformExt(Constants.LoginServerExe)))
           && File.Exists(Path.Combine(AurigaPath, Constants.WithPlatformExt(Constants.CharServerExe)))
           && File.Exists(Path.Combine(AurigaPath, Constants.WithPlatformExt(Constants.MapServerExe)))
           && File.Exists(ClientExePath);

    private void SetStatus(string msg, bool error)
    {
        StatusMessage = msg;
        StatusIsError = error;
    }

    [RelayCommand]
    public void Save()
    {
        try
        {
            if (!CanSave) return; // 条件スキップはログしない

            var s = new Models.AppSettings
            {
                AurigaPath = AurigaPath,
                ClientExePath = ClientExePath
            };
            s.Save();
            Services.LogService.Info("Settings", "設定を保存しました");
            CloseWindowRequested?.Invoke();
        }
        catch (Exception ex)
        {
            Services.LogService.Error("Settings", "設定の保存で例外が発生しました", ex);
            SetStatus("設定の保存に失敗しました。ログを確認してください。", error: true);
        }
    }
}
