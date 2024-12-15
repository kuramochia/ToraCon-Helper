using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

// ウィンカーが出ているときにハンドルを指定角度まで回したらウィンカーを消すアクション
public class BlinkerHideOnSteeringAction : TelemetryActionBase
{
    /// <summary>
    /// ウィンカーを消すまでの指定角度（0~1）マイナスはダメ
    /// </summary>
    public float BlinkerHidePosition { get; set; } = 1f;

    private float? _maxSteering = null;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var changed = false;
        var currentSteering = telemetry.ControlValues.GameValues.Steering;

        // 前回のステアリング値よりも大きくなった＝もっとハンドル切ってる
        var left = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerLeftActive;
        var right = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerRightActive;

        // ウィンカー出てないので、監視解除
        if(!left && !right)
        {
            _maxSteering = null;
            return changed;
        }

        if ((left || right) && _maxSteering == null)
        {
            // 監視開始
            //Debug.WriteLine($"監視開始 current={currentSteering}");
            _maxSteering = currentSteering;
            return changed;
        }

        // オートキャンセル監視中に
        if (_maxSteering != null)
        {
            //Debug.WriteLine($"監視中 current={currentSteering},_maxSteering={_maxSteering}, BlinkerHidePosition={BlinkerHidePosition} ");
            if (left)
            {
                // 左・閾値を下回った
                if (_maxSteering.Value - BlinkerHidePosition >= currentSteering)
                {
                    //Debug.WriteLine($"閾値以下 左 current={currentSteering}");
                    // 左ウィンカー戻す
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetLeftBlinkerHide();
                    _maxSteering = null;
                    changed = true;
                }
                else if(_maxSteering.Value <= currentSteering)
                {
                    _maxSteering = currentSteering;
                }
            }
            else if (right)
            {
                // 右・閾値を下回った
                if (_maxSteering.Value + BlinkerHidePosition <= currentSteering)
                {
                    //Debug.WriteLine($"閾値以下 右 current={currentSteering}");
                    // 右ウィンカー戻す
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetRightBlinkerHide();
                    _maxSteering = null;
                    changed = true;
                }
                else if (_maxSteering.Value >= currentSteering)
                {
                    _maxSteering = currentSteering;
                }
            }
        }
        return changed;
    }
}
