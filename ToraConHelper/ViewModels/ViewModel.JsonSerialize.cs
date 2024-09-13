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

            BlinkerLikeRealCarActionEnabled = BlinkerLikeRealCarActionEnabled,
            RetarderAllReduceActionEnabled = ReterderAllReduceActionEnabled,
            RetarderAllReduceActionLimitSpeedKph = ReterderAllReduceActionLimitSpeedKph,
            BlinkerHideOnSteeringActionEnabled = BlinkerHideOnSteeringActionEnabled,
            SteeringRotationAngle = SteeringRotationAngle,
            BlinkerHideBySteeringAngle = BlinkerHideBySteeringAngle,
            RetarderFullOnActionEnabled = RetarderFullOnActionEnabled,
            RetarderFullOffActionEnabled = RetarderFullOffActionEnabled
        };
        return s;
    }

    private void LoadFromSettings(ISettingFileMamager sm)
    {
        var s = sm.Load();
        IsActive = s.IsActive;
        GoToTasktrayOnAppClose = s.GoToTasktrayOnAppClose;
        TaskTrayOnStart = s.TaskTrayOnStart;

        BlinkerLikeRealCarActionEnabled = s.BlinkerLikeRealCarActionEnabled;
        ReterderAllReduceActionEnabled = s.RetarderAllReduceActionEnabled;
        ReterderAllReduceActionLimitSpeedKph = s.RetarderAllReduceActionLimitSpeedKph;
        BlinkerHideOnSteeringActionEnabled = s.BlinkerHideOnSteeringActionEnabled;
        SteeringRotationAngle = s.SteeringRotationAngle;
        BlinkerHideBySteeringAngle = s.BlinkerHideBySteeringAngle;
        RetarderFullOnActionEnabled = s.RetarderFullOnActionEnabled;
        RetarderFullOffActionEnabled = s.RetarderFullOffActionEnabled;
    }
}
