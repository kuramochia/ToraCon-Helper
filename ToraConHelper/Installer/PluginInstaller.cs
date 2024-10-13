using System;
using System.Diagnostics;
using System.IO;
using ToraConHelper.Helpers;

namespace ToraConHelper.Installer;

internal class PluginInstaller
{
    const string PluginPath = "plugins";
    const string PluginFile = "ToraCon-scs-telemetry.dll";
    const string CopySourcePath = $".\\plugins\\win_x64\\{PluginFile}";

    internal event EventHandler<AddMessageEventArgs>? AddMessageFromInstaller;

    internal bool NeedInstall() => InstallProcess(true);
    internal bool InstallProcess(bool onlyCheck = false)
    {
        AddMessage("Telemetry DLL インストールプロセス開始");
        AddMessage("");
        // ゲーム フォルダを探す
        var paths = SteamHelper.DetectGamePaths();
        foreach (var path in paths)
        {
            var p = path.Path;
            try
            {
                AddMessage($"ゲームが見つかりました : {p}");

                // プラグインフォルダを探す、無ければ作る
                var exePath = SteamHelper.GetExePath(p);
                if (!Directory.Exists(exePath))
                {
                    AddMessage($"ゲームフォルダが見つかりませんでした : {exePath}");
                }
                else
                {
                    var pluginFolder = Path.Combine(exePath, PluginPath);
                    // Plugin フォルダは無ければ作る
                    if (!Directory.Exists(pluginFolder))
                    {
                        if (onlyCheck) return true;
                        Directory.CreateDirectory(pluginFolder);
                        AddMessage($"plugins フォルダが見つからないため、作成しました : {pluginFolder}");
                    }

                    // Telemetry DLL のファイルバージョン確認
                    var pluginFilePath = Path.Combine(pluginFolder, PluginFile);
                    var needCopy = false;
                    if (!File.Exists(pluginFilePath))
                    {
                        // file not found
                        AddMessage($"{PluginFile} が見つかりませんでした");
                        needCopy = true;
                    }
                    else
                    {
                        // file found
                        // check version
                        Version currentVersion = new(FileVersionInfo.GetVersionInfo(CopySourcePath).FileVersion);
                        if (!Version.TryParse(FileVersionInfo.GetVersionInfo(pluginFilePath).FileVersion, out Version telemetryVersion))
                        {
                            // バージョン番号がない時の DLL なので更新
                            AddMessage($"{PluginFile} の更新が必要です : 0.0.0.0 → {currentVersion}");
                            needCopy = true;
                        }
                        else if (telemetryVersion < currentVersion)
                        {
                            // 更新が必要
                            AddMessage($"{PluginFile} の更新が必要です : {telemetryVersion} → {currentVersion}");
                            needCopy = true;
                        }
                        else
                        {
                            AddMessage($"{PluginFile} は既に最新バージョンがインストールされています : {telemetryVersion}");
                        }
                    }

                    if (onlyCheck && needCopy) return needCopy;
                    if (needCopy)
                    {
                        File.Copy(CopySourcePath, pluginFilePath, true);
                        AddMessage($"{PluginFile} のコピーが完了しました");
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessage($"エラーが発生しました");
                AddMessage($"お手数ですが、手動で {PluginFile} のコピーをお願いいたします");
                AddMessage(ex.ToString());
                AddMessage($"");
                if (onlyCheck) return true;
            }
            finally
            {
                AddMessage("");
            }
        }
        AddMessage($"Telemetry DLL インストールプロセス終了");
        AddMessage("このウィンドウを閉じてください");
        return false;
    }

    void AddMessage(string message) => AddMessageFromInstaller?.Invoke(this, new AddMessageEventArgs(message));
}

internal class AddMessageEventArgs : EventArgs
{
    internal AddMessageEventArgs(string message)
    {
        Message = message;
    }
    internal string Message { get; private set; }

}