using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public class ReterderAllReduceOnThrottleAction : TelemetryActionBase
{
    private const float LimitThrottle = 0.1f; // スロットルの閾値
    private uint _reterderLevel = 0;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var changed = false;
        var reterderLevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
        var currentAccel = telemetry.ControlValues.InputValues.Throttle;

        //Debug.WriteLine($"ReterderAllReduceOnThrottleAction: reterderLevel={reterderLevel}, currentAccel={currentAccel}");

        // リターダー段数を減らしてる
        // スロットルがONの場合
        if (_reterderLevel > 0 && _reterderLevel > reterderLevel && currentAccel >= LimitThrottle)
        {
            // リターダー段数0を入力する
            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(0);
            _reterderLevel = 0;
            changed = true;
        }
        else
        {
            _reterderLevel = reterderLevel;
        }
        return changed;
    }
}