using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

public class BlinkerForLaneChangeAction : TelemetryActionBase
{
    public float LowerLimitKph { get; set; } 
    public float SteeringLimit { get; set; } 

    public TimeSpan OffTime { get; set; }

    private bool _isShownBlinker = false;
    private bool _isWatching = false;
    private long _limitTicks = 0;
    private (float Min, float Max) _steeringLimitPair = default;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        // 前回のステアリング値よりも大きくなった＝もっとハンドル切ってる
        var left = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerLeftActive;
        var right = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerRightActive;

        var blinkerCurrentlyOn = false;

        // ウィンカー出てる
        if (left || right)
        {
            // 今まで出てなかった
            if (!_isShownBlinker)
            {
                // 今回出した
                blinkerCurrentlyOn = true;
            }
            _isShownBlinker = true;
        }
        // ウィンカー消えてる
        else if (!left && !right)
        {
            // 今まで出てた
            _isShownBlinker = false;
        }

        var currentSpeedKph = telemetry.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
        var currentSteering = telemetry.ControlValues.GameValues.Steering;

        if (!_isWatching && blinkerCurrentlyOn && currentSpeedKph >= LowerLimitKph)
        {
            // 監視開始
            _isWatching = true;
            _limitTicks = DateTime.Now.Ticks + OffTime.Ticks;
            _steeringLimitPair = new() { Min = currentSteering - SteeringLimit, Max = currentSteering + SteeringLimit };
            Debug.WriteLine($"監視開始 L:{left}, R:{right} Speed:{currentSpeedKph}");
        }
        else if (_isWatching)
        {
            Debug.WriteLine($"監視中 L:{left}, R:{right} Speed:{currentSpeedKph}");

            // 監視中にウィンカー消した
            if (!_isShownBlinker)
            {
                // 監視終了
                _isWatching = false;
                //Debug.WriteLine("監視終了 ウィンカー消した");
            }
            // 指定速度を下回った
            else if (currentSpeedKph < LowerLimitKph)
            {
                // 監視終了
                _isWatching = false;
                Debug.WriteLine($"監視終了 指定速度を下回った Limit:{LowerLimitKph} Current:{currentSpeedKph}");
            }
            // ステアリングを設定以上に切った
            else if (_steeringLimitPair.Min > currentSteering || _steeringLimitPair.Max < currentSteering)
            {
                // 監視終了
                _isWatching = false;
                Debug.WriteLine($"監視終了 ステアリングを指定値より切った Limit:{_steeringLimitPair} Current:{currentSteering}");
            }
            else if (_limitTicks <= DateTime.Now.Ticks)
            {
                // 指定時間までウィンカー出てた
                // ウィンカー Off
                using var input = new SCSSdkTelemetryInput();
                input.Connect();
                if (left)
                {
                    input.SetLeftBlinkerHide();
                }
                else if (right)
                {
                    input.SetRightBlinkerHide();
                }
                _isWatching = false;
                Debug.WriteLine("監視終了 ウィンカー消した");
                return true;
            }
        }
        return false;
    }
}
