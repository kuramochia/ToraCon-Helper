using ToraConHelper.Services;

namespace ToraConHelper.ViewModels;

public partial class ViewModel
{
    // 面倒なんだけど、ViewModel をそのまま Serialize/Deserialize したくないので手作業。
    private Settings ToSettings()
    {
        var s = new Settings
        {
            IsActive = IsActive,
            GoToTasktrayOnAppClose = GoToTasktrayOnAppClose,
            TaskTrayOnStart = TaskTrayOnStart,
            MinimizeOnStart = MinimizeOnStart,
            LastShownPage = LastShownPage,

            BlinkerLikeRealCarActionEnabled = BlinkerLikeRealCarActionEnabled,
            RetarderAllReduceActionEnabled = RetarderAllReduceActionEnabled,
            RetarderAllReduceActionLimitSpeedKph = RetarderAllReduceActionLimitSpeedKph,
            BlinkerHideOnSteeringActionEnabled = BlinkerHideOnSteeringActionEnabled,
            SteeringRotationAngle = SteeringRotationAngle,
            BlinkerHideBySteeringAngle = BlinkerHideBySteeringAngle,
            RetarderFullOnActionEnabled = RetarderFullOnActionEnabled,
            RetarderFullOffActionEnabled = RetarderFullOffActionEnabled,
            EngineBrakeAutoOffActionEnabled = EngineBrakeAutoOffActionEnabled,
            EngineBrakeAutoOffActionLimitSpeedKph = EngineBrakeAutoOffActionLimitSpeedKph,
            RetarderAutoOffActionEnabled = RetarderAutoOffActionEnabled,
            RetarderAutoOffActionLimitSpeedKph = RetarderAutoOffActionLimitSpeedKph,
            AutoFullFuelEnabled = AutoFullFuelEnabled,
            BlinkerForLaneChangeEnabled = BlinkerForLaneChangeEnabled,
            BlinkerForLaneChangeLimitSpeedKph = BlinkerForLaneChangeLimitSpeedKph,
            BlinkerForLaneChangeOffSeconds = BlinkerForLaneChangeOffSeconds,
            BlinkerForLaneChangeSteeringAngle = BlinkerForLaneChangeSteeringAngle,
            BlinkerLikeRealCarDInputActionEnabled = BlinkerLikeRealCarDInputActionEnabled,
            BlinkerDInputJoyStickType = BlinkerDInputJoyStickType,
            RetarderSkipInputActionEnabled = RetarderSkipInputActionEnabled,
            RetarderSkipInputLevel = RetarderSkipInputLevel,
            RetarderAllReduceOnThrottleEnabled = RetarderAllReduceOnThrottleEnabled,
            FollowSpeedLimitCruiseControlEnabled = FollowSpeedLimitCruiseControlEnabled,
            CruiseControlMPHinATS = CruiseControlMPHinATS,
            CruiseControlStep = CruiseControlStep,
            AutoFlasherAtReverseActionEnabled = AutoFlasherAtReverseActionEnabled,
            IgnoreFlasherOffWhenReverseOff = IgnoreFlasherOffWhenReverseOff,
        };
        s.PowerToysSettings ??= [];
        s.PowerToysSettings.Add(GameType.ETS2, Ets2!.ToSettings());
        s.PowerToysSettings.Add(GameType.ATS, Ats!.ToSettings());

        return s;
    }

    private void LoadFromSettings(ISettingFileManager sm)
    {
        var s = sm.Load();
        IsActive = s.IsActive;
        GoToTasktrayOnAppClose = s.GoToTasktrayOnAppClose;
        TaskTrayOnStart = s.TaskTrayOnStart;
        MinimizeOnStart = s.MinimizeOnStart;
        LastShownPage = s.LastShownPage;

        BlinkerLikeRealCarActionEnabled = s.BlinkerLikeRealCarActionEnabled;
        RetarderAllReduceActionEnabled = s.RetarderAllReduceActionEnabled;
        RetarderAllReduceActionLimitSpeedKph = s.RetarderAllReduceActionLimitSpeedKph;
        BlinkerHideOnSteeringActionEnabled = s.BlinkerHideOnSteeringActionEnabled;
        SteeringRotationAngle = s.SteeringRotationAngle;
        BlinkerHideBySteeringAngle = s.BlinkerHideBySteeringAngle;
        RetarderFullOnActionEnabled = s.RetarderFullOnActionEnabled;
        RetarderFullOffActionEnabled = s.RetarderFullOffActionEnabled;
        EngineBrakeAutoOffActionEnabled = s.EngineBrakeAutoOffActionEnabled;
        EngineBrakeAutoOffActionLimitSpeedKph = s.EngineBrakeAutoOffActionLimitSpeedKph;
        RetarderAutoOffActionEnabled = s.RetarderAutoOffActionEnabled;
        RetarderAutoOffActionLimitSpeedKph = s.RetarderAutoOffActionLimitSpeedKph;
        AutoFullFuelEnabled = s.AutoFullFuelEnabled;
        BlinkerForLaneChangeEnabled = s.BlinkerForLaneChangeEnabled;
        BlinkerForLaneChangeLimitSpeedKph = s.BlinkerForLaneChangeLimitSpeedKph;
        BlinkerForLaneChangeOffSeconds = s.BlinkerForLaneChangeOffSeconds;
        BlinkerForLaneChangeSteeringAngle = s.BlinkerForLaneChangeSteeringAngle;
        BlinkerLikeRealCarDInputActionEnabled = s.BlinkerLikeRealCarDInputActionEnabled;
        BlinkerDInputJoyStickType = s.BlinkerDInputJoyStickType;
        RetarderSkipInputActionEnabled = s.RetarderSkipInputActionEnabled;
        RetarderSkipInputLevel = s.RetarderSkipInputLevel;
        RetarderAllReduceOnThrottleEnabled = s.RetarderAllReduceOnThrottleEnabled;
        FollowSpeedLimitCruiseControlEnabled = s.FollowSpeedLimitCruiseControlEnabled;
        CruiseControlMPHinATS = s.CruiseControlMPHinATS;
        CruiseControlStep = s.CruiseControlStep;
        AutoFlasherAtReverseActionEnabled = s.AutoFlasherAtReverseActionEnabled;
        IgnoreFlasherOffWhenReverseOff = s.IgnoreFlasherOffWhenReverseOff;

        Ets2 = PowerToysViewModel.FromSettings(this, GameType.ETS2, s.PowerToysSettings?[GameType.ETS2]);
        Ats = PowerToysViewModel.FromSettings(this, GameType.ATS, s.PowerToysSettings?[GameType.ATS]);
    }
}
