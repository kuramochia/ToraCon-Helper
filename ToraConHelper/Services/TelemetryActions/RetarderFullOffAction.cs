using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

/// <summary>
/// リターダーフルから1段戻したら全段戻す
/// </summary>
public class RetarderFullOffAction : TelemetryActionBase
{
    private uint _currentRetarderLevel = 0;

    public override void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var stepCount = telemetry.TruckValues.ConstantsValues.MotorValues.RetarderStepCount;
        var retarderlevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;

        // リターダーが全段から１段戻ったら
        if (_currentRetarderLevel == stepCount && retarderlevel == stepCount - 1)
        {
            // リターダー全段戻す
            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(0);
        }
        _currentRetarderLevel = retarderlevel;
    }
}