using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public class RetarderAllReduceOnThrottleAction : TelemetryActionBase
{
    private const float LimitThrottle = 0.1f; // スロットルの閾値
    private uint _retarderLevel = 0;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var changed = false;
        var retarderLevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
        var currentAccel = telemetry.ControlValues.InputValues.Throttle;

        //Debug.WriteLine($"RetarderAllReduceOnThrottleAction: retarderLevel={retarderLevel}, currentAccel={currentAccel}");

        // リターダー段数を減らしてる
        // スロットルがONの場合
        if (_retarderLevel > 0 && _retarderLevel > retarderLevel && currentAccel >= LimitThrottle)
        {
            // リターダー段数0を入力する
            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(0);
            _retarderLevel = 0;
            changed = true;
        }
        else
        {
            _retarderLevel = retarderLevel;
        }
        return changed;
    }
}
