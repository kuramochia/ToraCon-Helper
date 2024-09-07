using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public class ReterderAllReduceAction : ITelemetryAction
{
    /// <summary>
    /// リターダーを前段戻す速度
    /// </summary>
    public int LimitSpeedKph { get; set; } = 30;

    private int _reterderLevel = 0;

    public void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var reterderLevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
        // リターダー段数が、前と変わらないか、前よりも大きい場合
        if (_reterderLevel <= reterderLevel)
        {
            // 値を取得
            _reterderLevel = (int)reterderLevel;
        }
        else if (reterderLevel > 0)
        {
            // リターダー段数を減らしてる
            var currentSpeedKph = telemetry.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
            // 速度が制限速度以下の場合
            if (currentSpeedKph <= LimitSpeedKph)
            {
                // リターダー段数0を入力する
                using var input = new SCSSdkTelemetryInput();
                input.Connect();
                input.SetRetarder(0);
                _reterderLevel = 0;
            }
        }
    }
}
