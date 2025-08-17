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
            RetarderAllReduceActionEnabled = ReterderAllReduceActionEnabled,
            RetarderAllReduceActionLimitSpeedKph = ReterderAllReduceActionLimitSpeedKph,
            BlinkerHideOnSteeringActionEnabled = BlinkerHideOnSteeringActionEnabled,
            SteeringRotationAngle = SteeringRotationAngle,
            BlinkerHideBySteeringAngle = BlinkerHideBySteeringAngle,
            RetarderFullOnActionEnabled = RetarderFullOnActionEnabled,
            RetarderFullOffActionEnabled = RetarderFullOffActionEnabled,
            EngineBrakeAutoOffActionEnabled = EngineBrakeAutoOffActionEnabled,
            EngineBrakeAutoOffActionLimitSpeedKph = EngineBrakeAutoOffActionLimitSpeedKph,
            ReterderAutoOffActionEnabled = ReterderAutoOffActionEnabled,
            ReterderAutoOffActionLimitSpeedKph = ReterderAutoOffActionLimitSpeedKph,
            AutoFullFuelEnabled = AutoFullFuelEnabled,
            BlinkerForLaneChangeEnabled = BlinkerForLaneChangeEnabled,
            BlinkerForLaneChangeLimitSpeedKph = BlinkerForLaneChangeLimitSpeedKph,
            BlinkerForLaneChangeOffSeconds = BlinkerForLaneChangeOffSeconds,
            BlinkerForLaneChangeSteeringAngle = BlinkerForLaneChangeSteeringAngle,
            BlinkerLikeRealCarDInputActionEnabled = BlinkerLikeRealCarDInputActionEnabled,
            BlinkerDInputJoyStickType = BlinkerDInputJoyStickType,
            ReterderSkipInputActionEnabled = ReterderSkipInputActionEnabled,
            ReterderSkipInputLevel = ReterderSkipInputLevel,
            ReterderAllReduceOnThrottleEnabled = ReterderAllReduceOnThrottleEnabled,
        };
        s.PowerToysSettings ??= [];
        s.PowerToysSettings.Add(GameType.ETS2, Ets2!.ToSettings());
        s.PowerToysSettings.Add(GameType.ATS, Ats!.ToSettings());

        return s;
    }

    private void LoadFromSettings(ISettingFileMamager sm)
    {
        var s = sm.Load();
        IsActive = s.IsActive;
        GoToTasktrayOnAppClose = s.GoToTasktrayOnAppClose;
        TaskTrayOnStart = s.TaskTrayOnStart;
        MinimizeOnStart = s.MinimizeOnStart;
        LastShownPage = s.LastShownPage;

        BlinkerLikeRealCarActionEnabled = s.BlinkerLikeRealCarActionEnabled;
        ReterderAllReduceActionEnabled = s.RetarderAllReduceActionEnabled;
        ReterderAllReduceActionLimitSpeedKph = s.RetarderAllReduceActionLimitSpeedKph;
        BlinkerHideOnSteeringActionEnabled = s.BlinkerHideOnSteeringActionEnabled;
        SteeringRotationAngle = s.SteeringRotationAngle;
        BlinkerHideBySteeringAngle = s.BlinkerHideBySteeringAngle;
        RetarderFullOnActionEnabled = s.RetarderFullOnActionEnabled;
        RetarderFullOffActionEnabled = s.RetarderFullOffActionEnabled;
        EngineBrakeAutoOffActionEnabled = s.EngineBrakeAutoOffActionEnabled;
        EngineBrakeAutoOffActionLimitSpeedKph = s.EngineBrakeAutoOffActionLimitSpeedKph;
        ReterderAutoOffActionEnabled = s.ReterderAutoOffActionEnabled;
        ReterderAutoOffActionLimitSpeedKph = s.ReterderAutoOffActionLimitSpeedKph;
        AutoFullFuelEnabled = s.AutoFullFuelEnabled;
        BlinkerForLaneChangeEnabled = s.BlinkerForLaneChangeEnabled;
        BlinkerForLaneChangeLimitSpeedKph = s.BlinkerForLaneChangeLimitSpeedKph;
        BlinkerForLaneChangeOffSeconds = s.BlinkerForLaneChangeOffSeconds;
        BlinkerForLaneChangeSteeringAngle = s.BlinkerForLaneChangeSteeringAngle;
        BlinkerLikeRealCarDInputActionEnabled = s.BlinkerLikeRealCarDInputActionEnabled;
        BlinkerDInputJoyStickType = s.BlinkerDInputJoyStickType;
        ReterderSkipInputActionEnabled = s.ReterderSkipInputActionEnabled;
        ReterderSkipInputLevel = s.ReterderSkipInputLevel;
        ReterderAllReduceOnThrottleEnabled = s.ReterderAllReduceOnThrottleEnabled;

        Ets2 = PowerToysViewModel.FromSettings(this, GameType.ETS2, s.PowerToysSettings?[GameType.ETS2]);
        Ats = PowerToysViewModel.FromSettings(this, GameType.ATS, s.PowerToysSettings?[GameType.ATS]);
    }
}
