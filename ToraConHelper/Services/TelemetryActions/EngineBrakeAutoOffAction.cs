using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

internal class EngineBrakeAutoOffAction : ITelemetryAction
{
    /// <summary>
    /// エンジンブレーキをオフにする速度
    /// </summary>
    public int LimitSpeedKph { get; set; }

    public void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var currentSpeedKph = telemetry.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
        if (currentSpeedKph <= LimitSpeedKph)
        {
            var currentBrake = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.MotorBrake;
            if (currentBrake)
            {
                using var input = new SCSSdkTelemetryInput();
                input.Connect();
                input.SetEngineBrakeToggle();
            }
        }
    }
}
