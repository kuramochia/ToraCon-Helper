using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
    [ObservableProperty]
    private PowerToysViewModel? ets2;

    partial void OnEts2Changed(PowerToysViewModel? oldValue, PowerToysViewModel? newValue) => oldValue?.Dispose();

    [ObservableProperty]
    private PowerToysViewModel? ats;

    partial void OnAtsChanged(PowerToysViewModel? oldValue, PowerToysViewModel? newValue) => oldValue?.Dispose();

    private void GameProcessDetector_GameProcessEnded(object sender, EventArgs e)
    {
        // Game process has ended, reset some properties
        GameTime = null;
        GameName = null;
        NavigationDistance = null;
        NavigationTime = null;
        TruckName = null;
        CargoName = null;
        NextRestStopTime = null;
        NextMandatoryBreakTime = null;
        lastRenderTimestamp = 0;
    }

    private static readonly HashSet<string> IgnoreSaveTargetNames = new(new[]
    {
        nameof(GameTime),
        nameof(GameName),
        nameof(NavigationDistance),
        nameof(NavigationTime),
        nameof(TruckName),
        nameof(CargoName),
        nameof(NextRestStopTime),
        nameof(NextMandatoryBreakTime)
    }, StringComparer.InvariantCultureIgnoreCase);

    private bool IsSaveTargetName(string propertyName) => !IgnoreSaveTargetNames.Contains(propertyName);

    internal void Save()
    {
        if (!isInitialization) settingFile.Save(ToSettings());
    }


    #region Powertoys Page の情報表示用

    private ulong lastRenderTimestamp = 0;

    // Powertoys Page の情報表示用
    private void GameInfoAction_GameInfoUpdated(object sender, GameInfoUpdatedEventArgs e)
    {
        // 500ms に 1 回だけ更新
        if (lastRenderTimestamp + (500 * 1000) <= e.Telemetry.RenderTimestamp)
            lastRenderTimestamp = e.Telemetry.RenderTimestamp;
        else return;
        Debug.WriteLine($"GameInfoAction_GameInfoUpdated: RenderTimestamp={e.Telemetry.RenderTimestamp}");

        GameTime = e.Telemetry.CommonValues.GameTime.Date;
        GameName = e.Telemetry.Game.ToString().ToUpper();
        NavigationDistance = e.Telemetry.NavigationValues.NavigationDistance / 1000; // m to km
        NavigationTime = TimeSpan.FromSeconds(e.Telemetry.NavigationValues.NavigationTime);

        TruckName = $"{e.Telemetry.TruckValues.ConstantsValues.Brand} {e.Telemetry.TruckValues.ConstantsValues.Name}";
        CargoName = e.Telemetry.JobValues.CargoValues.Name;

        NextRestStopTime = TimeSpan.FromMinutes(e.Telemetry.CommonValues.NextRestStopTime.Value - e.Telemetry.CommonValues.GameTime.Value);
        NextMandatoryBreakTime = TimeSpan.FromMinutes(e.Telemetry.CommonValues.NextMandatoryBreakTime.Value - e.Telemetry.CommonValues.GameTime.Value);
    }

    // ゲーム内時間
    [ObservableProperty]
    private DateTime? gameTime;

    // ゲーム名
    [ObservableProperty]
    private string? gameName;

    [ObservableProperty]
    private string? truckName;

    [ObservableProperty]
    private float? navigationDistance;

    [ObservableProperty]
    private TimeSpan? navigationTime;

    [ObservableProperty]
    private string? cargoName;

    [ObservableProperty]
    private TimeSpan? nextRestStopTime;

    [ObservableProperty]
    private TimeSpan? nextMandatoryBreakTime;

    #endregion

}
