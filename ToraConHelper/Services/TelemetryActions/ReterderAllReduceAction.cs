using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public class ReterderAllReduceAction : TelemetryActionBase
{
    /// <summary>
    /// リターダーを全段戻す速度
    /// </summary>
    public int LimitSpeedKph { get; set; } = 30;

    private uint _reterderLevel = 0;

    public override void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var reterderLevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
        var currentSpeedKph = telemetry.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
        // リターダー段数を減らしてる
        // 速度が制限速度以下の場合
        if (_reterderLevel > 0 && _reterderLevel > reterderLevel && currentSpeedKph <= LimitSpeedKph)
        {
            // リターダー段数0を入力する
            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(0);
            _reterderLevel = 0;
        }
        else
        {
            _reterderLevel = reterderLevel;
        }
    }
}
