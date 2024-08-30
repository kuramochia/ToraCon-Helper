using SCSSdkClient.Input;
using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public class BlinkerLikeRealCarAction : ITelemetryAction
{
    private bool _blinkerLeft, _blinkerRight;

    public void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        // 左右ウィンカーがアクティブかどうか（ハザードランプには反応しない）
        var left = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerLeftActive;
        var right = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerRightActive;

        // 今までウィンカー出てなかったけど出た ＝ 新しくウィンカーつけた
        if (!_blinkerLeft && !_blinkerRight)
        {
            if (left || right)
            {
                _blinkerLeft = left;
                _blinkerRight = right;
            }
        }
        // すでにウィンカー出てた
        else
        {
            // 左が出てた状態から
            if (_blinkerLeft)
            {
                // 右押された
                if (right)
                {
                    // 右消す入力
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetRightBlinker();
                    _blinkerLeft = false;
                    _blinkerRight = false;
                }

            }
            // 右が出てた状態から
            else if (_blinkerRight)
            {
                // 左押された
                if (left)
                {
                    // 左消す入力
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetLeftBlinker();
                    _blinkerLeft = false;
                    _blinkerRight = false;
                }
            }
        }
    }

}
