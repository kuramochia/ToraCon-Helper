using SCSSdkClient.Object;
using System;

namespace ToraConHelper.Services.TelemetryActions;

public class GameInfoAction : TelemetryActionBase
{
    public event EventHandler<GameInfoUpdatedEventArgs>? GameInfoUpdated;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        GameInfoUpdated?.Invoke(this, new(telemetry));
        return false;
    }
}

public class GameInfoUpdatedEventArgs : EventArgs
{
    public SCSTelemetry Telemetry { get; }
    public GameInfoUpdatedEventArgs(SCSTelemetry telemetry)
    {
        Telemetry = telemetry;
    }
}

