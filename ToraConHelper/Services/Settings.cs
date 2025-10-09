using System.Collections.Generic;
using ToraConHelper.Helpers;
using ToraConHelper.ViewModels;

namespace ToraConHelper.Services;

public class Settings
{
    public bool IsActive { get; set; } = true;
    public bool GoToTasktrayOnAppClose { get; set; } = false;
    public bool TaskTrayOnStart { get; set; } = false;
    public bool MinimizeOnStart { get; set; } = false;
    public string? LastShownPage { get; set; } = null;

    public bool BlinkerLikeRealCarActionEnabled { get; set; } = false;
    public bool BlinkerLikeRealCarDInputActionEnabled { get; set; } = true;
    public bool BlinkerHideOnSteeringActionEnabled { get; set; } = false;
    public int BlinkerHideBySteeringAngle { get; set; } = 90;
    public bool BlinkerForLaneChangeEnabled { get; set; } = false;
    public int BlinkerForLaneChangeLimitSpeedKph { get; set; } = 40;
    public int BlinkerForLaneChangeSteeringAngle { get; set; } = 25;
    public int BlinkerForLaneChangeOffSeconds { get; set; } = 5;

    public int SteeringRotationAngle { get; set; } = 1800;

    public bool RetarderAllReduceActionEnabled { get; set; } = false;
    public int RetarderAllReduceActionLimitSpeedKph { get; set; } = 30;

    public bool RetarderFullOnActionEnabled { get; set; } = false;
    public bool RetarderFullOffActionEnabled { get; set; } = false;

    public bool ReterderAutoOffActionEnabled { get; set; } = false;

    public int ReterderAutoOffActionLimitSpeedKph { get; set; } = 10;

    public bool ReterderSkipInputActionEnabled { get; set; } = false;

    public int ReterderSkipInputLevel { get; set; } = 1;

    public bool ReterderAllReduceOnThrottleEnabled { get; set; } = false;

    public bool EngineBrakeAutoOffActionEnabled { get; set; } = false;

    public int EngineBrakeAutoOffActionLimitSpeedKph { get; set; } = 10;

    public bool FollowSpeedLimitCruiseControlEnabled { get; set; } = false;

    public bool CruiseControlMPHinATS { get; set; } = true;

    public int CruiseControlStep { get; set; } = 5;


    public bool AutoFullFuelEnabled { get; set; } = true;
    public BlinkerJoyStickType BlinkerDInputJoyStickType { get; set; } = BlinkerJoyStickType.LeftStick;

    public Dictionary<GameType, PowerToysSettings>? PowerToysSettings { get; set; }
}

public class PowerToysSettings
{
    public string? GameDataFolder { get; set; }

}