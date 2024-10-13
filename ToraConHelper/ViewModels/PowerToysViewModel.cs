using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ToraConHelper.Services;

namespace ToraConHelper.ViewModels;

public partial class PowerToysViewModel : ObservableObject
{
    private ViewModel parentViewModel;

    private GameType gameType;
    private Uri gameUri;

    public PowerToysViewModel(GameType gameType, ViewModel parentViewModel)
    {
        this.gameType = gameType;
        this.parentViewModel = parentViewModel;
        gameUri = GetGameRunUri()!;
    }

    [RelayCommand]
    private void OpenGame() => Process.Start(gameUri.ToString());

    [ObservableProperty]
    private string? gameDataFolder;

    partial void OnGameDataFolderChanged(string? value)
    {
        parentViewModel.Save();

        // Update Profiles Data
        LocalProfiles = new(GetLocalProfiles());
        SteamProfiles = new(GetSteamProfiles());
    }

    #region OpenGameDataFolder
    [RelayCommand(CanExecute = nameof(CanOpenGameDataFolder))]
    private void OpenGameDataFolder() => Process.Start(GameDataFolder);

    private bool CanOpenGameDataFolder() => GameDataFolder != null && Directory.Exists(GameDataFolder);
    #endregion

    #region OpenGameLog
    private const string GAMELOG_FILE = "game.log.txt";
    [RelayCommand(CanExecute = nameof(CanOpenGameLog))]
    private void OpenGameLog() => Process.Start(Path.Combine(GameDataFolder, GAMELOG_FILE));

    private bool CanOpenGameLog() => GameDataFolder != null && File.Exists(Path.Combine(GameDataFolder, GAMELOG_FILE));
    #endregion

    #region OpenConfig
    private const string CONFIG_FILE = "config.cfg";
    [RelayCommand(CanExecute = nameof(CanOpenGameConfig))]
    private void OpenGameConfig() => Process.Start(Path.Combine(GameDataFolder, CONFIG_FILE));

    private bool CanOpenGameConfig() => GameDataFolder != null && File.Exists(Path.Combine(GameDataFolder, CONFIG_FILE));
    #endregion

    #region OpenModFolder
    private const string MOD_FOLDER = "mod";
    [RelayCommand(CanExecute = nameof(CanOpenModFolder))]
    private void OpenModFolder() => Process.Start(Path.Combine(GameDataFolder, MOD_FOLDER));

    private bool CanOpenModFolder() => GameDataFolder != null && Directory.Exists(Path.Combine(GameDataFolder, MOD_FOLDER));
    #endregion

    #region Profiles 
    [ObservableProperty]
    private ObservableCollection<ProfileFolderData> localProfiles = new();

    [ObservableProperty]
    private ObservableCollection<ProfileFolderData> steamProfiles = new();


    private IEnumerable<ProfileFolderData> GetLocalProfiles()
    {
        try
        {
            return Directory.EnumerateDirectories(Path.Combine(GameDataFolder, "profiles"))
                .Select(pf => new ProfileFolderData() { FullName = pf })
                .OrderByDescending(pfd => pfd.LastWriteTime);
        }
        catch {
            return Enumerable.Empty<ProfileFolderData>();
        }
    }

    private IEnumerable<ProfileFolderData> GetSteamProfiles()
    {
        try
        {
            return Directory.EnumerateDirectories(Path.Combine(GameDataFolder, "steam_profiles"))
                .Select(pf => new ProfileFolderData() { FullName = pf })
                .OrderByDescending(pfd => pfd.LastWriteTime);
        }
        catch
        {
            return Enumerable.Empty<ProfileFolderData>();
        }
    }
    #endregion

    #region Setialization Helper
    internal PowerToysSettings ToSettings()
    {
        PowerToysSettings result = new();
        result.GameDataFolder = GameDataFolder;
        return result;
    }

    static internal PowerToysViewModel FromSettings(ViewModel parentViewModel, GameType gameType, PowerToysSettings? settings)
    {
        PowerToysViewModel result = new(gameType, parentViewModel);
        result.GameDataFolder = settings != null ? settings.GameDataFolder : result.GetDefaultGameDataFolder();
        return result;
    }
    #endregion Setialization Helper

    internal string GetDefaultGameDataFolder()
    {
        switch (gameType)
        {
            case GameType.ETS2:
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Euro Truck Simulator 2");
            case GameType.ATS:
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "American Truck Simulator");
        }
        return string.Empty;
    }

    internal Uri? GetGameRunUri()
    {
        switch (gameType)
        {
            case GameType.ETS2:
                return new Uri("steam://run/227300");
            case GameType.ATS:
                return new Uri("steam://run/270880");
        }
        return null;
    }
}

public partial class ProfileFolderData : ObservableObject
{
    [ObservableProperty]
    private string? fullName;

    partial void OnFullNameChanged(string? value)
    {
        // プロファイルフォルダ名(エンコードされている)
        var dirInfo = new DirectoryInfo(value);
        this.Name = dirInfo.Name;
        this.LastWriteTime = dirInfo.LastWriteTime;
        try
        {
            var bytes = ConvertHexStringToByteArray(Name);
            // プロファイルフォルダ名(デコード済み)
            DecodedName = new UTF8Encoding().GetString(bytes);
        }
        catch
        {
            // デコード失敗したので、手動で作ったフォルダ
            DecodedName = Name;
        }
    }

    public static byte[] ConvertHexStringToByteArray(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? decodedName;

    [ObservableProperty]
    private DateTime lastWriteTime;

    [RelayCommand]
    void OpenFolder() => Process.Start(FullName);
}