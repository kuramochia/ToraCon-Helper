﻿namespace ToraConHelper.Services;

public class Settings
{
    public bool IsActive { get; set; } = true;
    public bool GoToTasktrayOnAppClose { get; set; } = false;
    public bool TaskTrayOnStart { get; set; } = false;

    public bool BlinkerLikeRealCarActionEnabled { get; set; } = true;
    public bool RetarderAllReduceActionEnabled { get; set; } = true;
    public int RetarderAllReduceActionLimitSpeedKph { get; set; } = 30;

    public bool BlinkerHideOnSteeringActionEnabled { get; set; } = false;

    public int SteeringRotationAngle { get; set; } = 1800;

    public int BlinkerHideBySteeringAngle { get; set; } = 90;

    public bool RetarderFullOnActionEnabled { get; set; } = false;
    public bool RetarderFullOffActionEnabled { get; set; } = false;
}