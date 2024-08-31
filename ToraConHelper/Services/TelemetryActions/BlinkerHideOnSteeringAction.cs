using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;

namespace ToraConHelper.Services.TelemetryActions;

// ウィンカーが出ているときにハンドルを指定角度まで回したらウィンカーを消すアクション
public class BlinkerHideOnSteeringAction : ITelemetryAction
{
    /// <summary>
    /// ウィンカーを消すまでの指定角度（0~1）マイナスはダメ
    /// </summary>
    public float BlinkerHidePosition { get; set; } = 1f;

    private float _maxSteering = 0f;

    public void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var currentSteering = telemetry.ControlValues.GameValues.Steering;

        // 前回のステアリング値よりも大きくなった＝もっとハンドル切ってる

        // ステアリング右方向
        if (currentSteering < 0)
        {
            // さらにステアリングを右に回した
            if (_maxSteering > currentSteering)
            {
                // 最大値更新
                _maxSteering = currentSteering;
            }
            // ステアリング左に戻した
            else
            {
                // 今回のステアリング値が、閾値よりも戻った
                if (Math.Abs(_maxSteering) - BlinkerHidePosition >= Math.Abs(currentSteering) &&
                    telemetry.TruckValues.CurrentValues.LightsValues.BlinkerRightActive)
                {
                    // 右ウィンカー戻す
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetRightBlinkerHide();
                    _maxSteering = currentSteering;
                }
                // そこまで戻ってない
                else
                {
                    // nop
                }
            }
        }
        else if (currentSteering > 0)
        {
            // ステアリング左方向
            // さらにステアリングを左に回した
            if (_maxSteering < currentSteering)
            {
                // 最大値更新
                _maxSteering = currentSteering;
            }
            // ステアリング左に戻した
            else
            {
                // 今回のステアリング値が、閾値よりも戻った
                if (_maxSteering - BlinkerHidePosition >= currentSteering &&
                    telemetry.TruckValues.CurrentValues.LightsValues.BlinkerLeftActive)
                {
                    // 左ウィンカー戻す
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetLeftBlinkerHide();
                    _maxSteering = currentSteering;
                }
                // そこまで戻ってない
                else
                {
                    // nop
                }
            }
        }
        else
        {
            _maxSteering = currentSteering;
        }
    }
}
