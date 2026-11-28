using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SCSSdkClient.Object;
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
        IsBusJob = false;
        BusJobMarketName = null;
        BusJobAverageSatisfaction = null;
        BusJobCurrentStage = null;
        BusJobStages = Array.Empty<BusJobStageInfo>();
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

        var busJob = e.Telemetry.BusJobValues;
        IsBusJob = e.Telemetry.SpecialEventsValues.OnBusJob;
        BusJobMarketName = busJob.MarketName;
        BusJobAverageSatisfaction = busJob.AverageSatisfaction;
        BusJobCurrentStage = busJob.CurrentStageIndex.HasValue
            ? $"{busJob.CurrentStageIndex.Value + 1} / {busJob.Stages.Count}"
            : null;
        BusJobStages = busJob.Stages
            .Select((stage, index) => new { Stage = stage, Index = index })
            // 現在のステージが分かる場合は、完了済みを除いて現在以降を表示する（インデックスは 0 始まり）。不明な場合は全件表示する。
            .Where(item => !busJob.CurrentStageIndex.HasValue || (uint)item.Index >= busJob.CurrentStageIndex.Value)
            .Select(item => new BusJobStageInfo(
                item.Stage,
                e.Telemetry.CommonValues.GameTime.Value,
                FormatGameTime(item.Stage.PlannedArrivalTime)))
            .ToArray();

        NextRestStopTime = TimeSpan.FromMinutes(Math.Max(0, e.Telemetry.CommonValues.NextRestStop.Value));
        NextMandatoryBreakTime = TimeSpan.FromMinutes(Math.Max(0, e.Telemetry.CommonValues.NextMandatoryBreak.Value));
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

    [ObservableProperty]
    private bool isBusJob;

    [ObservableProperty]
    private bool isInfoExpanded = true;

    [ObservableProperty]
    private string? busJobMarketName;

    [ObservableProperty]
    private float? busJobAverageSatisfaction;

    [ObservableProperty]
    private string? busJobCurrentStage;

    [ObservableProperty]
    private IReadOnlyList<BusJobStageInfo> busJobStages = Array.Empty<BusJobStageInfo>();

    private static string? FormatGameTime(uint? gameTime) =>
        gameTime.HasValue ? new SCSTelemetry.Time(gameTime.Value).Date.ToString("HH:mm") : null;

    #endregion
}

public sealed class BusJobStageInfo
{
    public string SourceName { get; }
    public string DestinationName { get; }
    public uint? PlannedEnteringPassengers { get; }
    public uint? PlannedLeavingPassengers { get; }
    public string? TimeUntilPlannedArrival { get; }
    public bool IsPastPlannedArrival { get; }
    public string? PlannedArrivalTime { get; }

    public BusJobStageInfo(
        SCSTelemetry.BusJob.Stage stage,
        uint currentTime,
        string? plannedArrivalTime)
    {
        SourceName = stage.SourceName;
        DestinationName = stage.DestinationName;
        PlannedEnteringPassengers = stage.PlannedEnteringPassengers;
        PlannedLeavingPassengers = stage.PlannedLeavingPassengers;
        PlannedArrivalTime = plannedArrivalTime;
        if (stage.PlannedArrivalTime.HasValue)
        {
            var difference = (long)currentTime - stage.PlannedArrivalTime.Value;
            IsPastPlannedArrival = difference > 0;
            var minutes = Math.Abs(difference);
            var sign = difference < 0 ? "-" : difference > 0 ? "+" : "";
            TimeUntilPlannedArrival = minutes >= 60
                ? $"{sign}{minutes / 60}時間{minutes % 60}分"
                : $"{sign}{minutes}分";
        }
    }
}
