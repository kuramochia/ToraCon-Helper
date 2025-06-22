using SCSSdkClient.Object;
using System;

namespace ToraConHelper.Services.TelemetryActions;

internal class GameTimeAction : TelemetryActionBase
{
    public delegate void GameTimeUpdatedDelegate(SCSTelemetry.Time gameTime);
    public event GameTimeUpdatedDelegate? GameTimeUpdated;

    private uint lastGameTime = uint.MinValue;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var gameTime = telemetry.CommonValues.GameTime;
        if (lastGameTime != gameTime.Value)
        {
            lastGameTime = gameTime.Value;
            GameTimeUpdated?.Invoke(gameTime);
        }
        return false;
    }
}

