using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

/// <summary>
/// リターダー1段入れたら全段入れる
/// </summary>
public class RetarderFullOnAction : ITelemetryAction
{
    private uint _currentRetarderLevel = 0;

    public void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var retarderlevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;

        // リターダーが0段から1段ｎ変わった
        if (_currentRetarderLevel == 0 && retarderlevel == 1)
        {
            // リターダーを Step Count まで入れる
            var stepCount = telemetry.TruckValues.ConstantsValues.MotorValues.RetarderStepCount;
            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(stepCount);
        }
        _currentRetarderLevel = retarderlevel;
    }
}
