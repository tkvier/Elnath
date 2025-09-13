// Services/ServerProcessManager.cs
// 日本語概要: login/char/map の3プロセスを起動・監視・再起動・終了。標準出力/標準エラーをイベントで購読し、UI に流します。
// エラーは LogService で記録。条件により処理スキップ（未設定・未起動）はログしません。
using System.Diagnostics;

namespace AurigaFrontend.Services;

public enum ServerKind { Login, Char, Map }

public class ProcessOutputEventArgs : EventArgs
{
    public required ServerKind Kind { get; init; }
    public required string Line { get; init; }
    public bool IsError { get; init; }
}

public class ServerProcessManager
{
    private readonly Dictionary<ServerKind, Process?> _procs = new();
    private readonly string _aurigaPath;

    public event EventHandler<ProcessOutputEventArgs>? OutputReceived;

    public ServerProcessManager(string aurigaPath)
    {
        _aurigaPath = aurigaPath;
        _procs[ServerKind.Login] = null;
        _procs[ServerKind.Char]  = null;
        _procs[ServerKind.Map]   = null;
    }

    private static string BuildExePath(string baseDir, string exeName)
        => Path.Combine(baseDir, AurigaFrontend.Constants.WithPlatformExt(exeName));

    private ProcessStartInfo CreateStartInfo(ServerKind kind)
    {
        var exe = kind switch
        {
            ServerKind.Login => BuildExePath(_aurigaPath, Constants.LoginServerExe),
            ServerKind.Char  => BuildExePath(_aurigaPath, Constants.CharServerExe),
            ServerKind.Map   => BuildExePath(_aurigaPath, Constants.MapServerExe),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };

        var psi = new ProcessStartInfo
        {
            FileName = exe,
            WorkingDirectory = _aurigaPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        return psi;
    }

    public bool IsRunning(ServerKind kind) => _procs[kind] != null && !_procs[kind]!.HasExited;

    public bool Start(ServerKind kind)
    {
        // 既に起動しているならスキップ（ログしない）
        if (IsRunning(kind)) return false;

        try
        {
            var psi = CreateStartInfo(kind);
            var p = new Process { StartInfo = psi, EnableRaisingEvents = true };

            p.OutputDataReceived += (_, e) =>
            {
                if (e.Data is null) return;
                OutputReceived?.Invoke(this, new ProcessOutputEventArgs { Kind = kind, Line = e.Data, IsError = false });
            };
            p.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is null) return;
                OutputReceived?.Invoke(this, new ProcessOutputEventArgs { Kind = kind, Line = e.Data, IsError = true });
            };

            p.Exited += (_, _) =>
            {
                OutputReceived?.Invoke(this, new ProcessOutputEventArgs { Kind = kind, Line = $"[{kind}] 終了しました (Code {p.ExitCode})", IsError = p.ExitCode != 0 });
                _procs[kind] = null;
            };

            if (!p.Start())
            {
                LogService.Error(kind.ToString(), "プロセスの起動に失敗しました");
                return false;
            }

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            _procs[kind] = p;
            LogService.Info(kind.ToString(), "起動しました");
            return true;
        }
        catch (Exception ex)
        {
            LogService.Error(kind.ToString(), "起動時に例外が発生しました", ex);
            return false;
        }
    }

    public bool Stop(ServerKind kind)
    {
        // 未起動ならスキップ（ログしない）
        if (!IsRunning(kind)) return false;

        try
        {
            var p = _procs[kind]!;
            p.EnableRaisingEvents = false;
            if (!p.HasExited)
            {
                p.CloseMainWindow(); // コンソールの場合効果がないこともある
                if (!p.WaitForExit(1500))
                {
                    p.Kill(true);
                    p.WaitForExit(3000);
                }
            }
            _procs[kind] = null;
            LogService.Info(kind.ToString(), "停止しました");
            return true;
        }
        catch (Exception ex)
        {
            LogService.Error(kind.ToString(), "停止時に例外が発生しました", ex);
            return false;
        }
    }

    public bool Restart(ServerKind kind)
    {
        var okStop = IsRunning(kind) ? Stop(kind) : true;
        if (!okStop)
        {
            // ここはエラー扱い（停止できなかった）
            LogService.Error(kind.ToString(), "再起動: 停止に失敗");
            return false;
        }
        return Start(kind);
    }
}
