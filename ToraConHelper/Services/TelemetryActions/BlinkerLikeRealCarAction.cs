using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

public class BlinkerLikeRealCarAction : TelemetryActionBase
{
    private bool _blinkerLeft, _blinkerRight;

    public override void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        // 左右ウィンカーがアクティブかどうか（ハザードランプには反応しない）
        var left = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerLeftActive;
        var right = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerRightActive;

        if(!left && !right)
        {
            // 両方出てない
            _blinkerLeft = left;
            _blinkerRight = right;
        }
        else
        {
            // どちらか出てる

            // 右出てた、左になった
            if (_blinkerRight && left)
            {
                // 左消す入力
                using var input = new SCSSdkTelemetryInput();
                input.Connect();
                input.SetLeftBlinkerHide();
                _blinkerLeft = false;
                _blinkerRight = false;
            }
            // 左出てた、右になった
            else if (_blinkerLeft && right)
            {
                // 右消す入力
                using var input = new SCSSdkTelemetryInput();
                input.Connect();
                input.SetRightBlinkerHide();
                _blinkerLeft = false;
                _blinkerRight = false;
            }
            else
            {
                _blinkerLeft = left;
                _blinkerRight = right;
            }
        }
    }
}
