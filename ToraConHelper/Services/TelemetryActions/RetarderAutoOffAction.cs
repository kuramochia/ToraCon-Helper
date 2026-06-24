using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public class RetarderAutoOffAction : TelemetryActionBase
{
    /// <summary>
    /// リターダーを自動的に戻す速度
    /// </summary>
    public int LimitSpeedKph { get; set; } = 10;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var changed = false;
        // 指定読度以下か
        var currentSpeedKph = telemetry.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
        if (currentSpeedKph <= LimitSpeedKph)
        {
            // リターダーが1段以上か
            var retarderLevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
            if (retarderLevel > 0)
            {
                // リターダー段数0を入力する
                using var input = new SCSSdkTelemetryInput();
                input.Connect();
                input.SetRetarder(0);
                changed = true;
            }
        }
        return changed;
    }
}
