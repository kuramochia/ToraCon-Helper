using Gameloop.Vdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToraConHelper.Installer;

internal class PluginInstaller
{
    readonly Dictionary<string, string> GameIds = new() {
        { /*ETS2*/"227300", "steamapps\\common\\Euro Truck Simulator 2" },
        { /*ATS*/"270880", "steamapps\\common\\American Truck Simulator" }
    };
    const string BinaryPath = "bin\\win_x64";
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
        var paths = DetectGamePaths();
        foreach (var p in paths)
        {
            try
            {
                AddMessage($"ゲームが見つかりました : {p}");

                // プラグインフォルダを探す、無ければ作る
                var exePath = Path.Combine(p, BinaryPath);
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

    string? GetSteamPath()
    {
        var steamKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
        return steamKey?.GetValue("SteamPath") as string;
    }

    IEnumerable<string> DetectGamePaths()
    {
        try
        {
            var vdfFile = Path.Combine(GetSteamPath(), "steamapps\\libraryfolders.vdf");
            dynamic vdf = VdfConvert.Deserialize(File.ReadAllText(vdfFile));

            var result = new List<string>();
            foreach (dynamic content in vdf.Value)
            {
                var basePath = content.Value.path.Value as string;
                var apps = content.Value.apps;
                foreach (dynamic app in apps)
                {
                    var detectedGames = GameIds.Where(id => id.Key == app.Key as string).Select(id => Path.Combine(basePath, id.Value));
                    result.AddRange(detectedGames);
                }
            }
            return result;
        }
        catch
        {
            return [];
        }
    }

}

internal class AddMessageEventArgs : EventArgs
{
    internal AddMessageEventArgs(string message)
    {
        Message = message;
    }
    internal string Message { get; private set; }

}