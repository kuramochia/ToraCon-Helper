using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public class RetarderAllReduceAction : TelemetryActionBase
{
    /// <summary>
    /// リターダーを全段戻す速度
    /// </summary>
    public int LimitSpeedKph { get; set; } = 30;

    private uint _retarderLevel = 0;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var changed = false;
        var retarderLevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
        var currentSpeedKph = telemetry.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
        // リターダー段数を減らしてる
        // 速度が制限速度以下の場合
        if (_retarderLevel > 0 && _retarderLevel > retarderLevel && currentSpeedKph <= LimitSpeedKph)
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
