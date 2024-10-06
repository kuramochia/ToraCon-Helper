using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;
using System.Linq;
using Vortice.DirectInput;

namespace ToraConHelper.Services.TelemetryActions;

/// <summary>
/// DirectInput の入力を拾って、ウィンカーを完全に実車っぽくする
/// </summary>
public class BlinkerLikeRealCarDInputAction : ITelemetryAction, IDisposable
{
    private DirectInputController _dInputController = new();

    public JoystickOffset LeftBlinkerJoyStick { get; set; }
    public JoystickOffset RightBlinkerJoyStick { get; set; }

    public void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        // Dinput 初期化
        if (!_dInputController.Initialize())
        {
            Debug.WriteLine("DirectInputController Initialize Failed.");
            return;
        }

        // Dinput の情報取得
        bool inputLeft = false, inputRight = false, hasUpdate = false;
        {
            var updates = _dInputController.GetBufferedJoystickData();
            if (updates != null)
            {
                inputLeft = updates.Any(u => u.Offset == LeftBlinkerJoyStick && u.Value > 0);
                inputRight = updates.Any(u => u.Offset == RightBlinkerJoyStick && u.Value > 0);
                if (inputLeft || inputRight)
                {
                    hasUpdate = true;
                    Debug.WriteLine($"{DateTime.Now} DInput GetFromBuffer Left={inputLeft}, Right={inputRight}");
                }
            }
        }

        // Dinput 更新アリ
        if (hasUpdate)
        {
            // 左右ウィンカーがアクティブかどうか（ハザードランプには反応しない）
            var left = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerLeftActive;
            var right = telemetry.TruckValues.CurrentValues.LightsValues.BlinkerRightActive;

            InputType inputType = InputType.None;

            if (inputLeft)
            {
                // 左入力、右出てた
                if (right)
                {
                    // 右消す入力
                    inputType = InputType.RightOff;
                }
                // 左入力、左出てた
                else if (left)
                {
                    // 左消す入力
                    inputType = InputType.LeftOff;
                }
                // 左入力、どっちも出てない
                else
                {
                    // 左出す入力
                    inputType = InputType.LeftOn;
                }
            }
            else if (inputRight)
            {
                // 右入力、左出てた
                if (left)
                {
                    // 左消す入力
                    inputType = InputType.LeftOff;
                }
                // 右入力、右出てた
                else if (right)
                {
                    // 右消す入力
                    inputType = InputType.RightOff;
                }
                // 右入力、どっちも出てない
                else
                {
                    // 右出す入力
                    inputType = InputType.RightOn;
                }
            }

            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            switch (inputType)
            {
                case InputType.LeftOn: { input.SetLeftBlinker(); break; }
                case InputType.RightOn: { input.SetRightBlinker(); break; }
                case InputType.LeftOff:
                    {
                        // ウィンカー出ている方向への入力で、レバー音がする状態で消せる
                        input.SetLeftBlinker();
                        break;
                    }
                case InputType.RightOff:
                    {
                        // ウィンカー出ている方向への入力で、レバー音がする状態で消せる
                        input.SetRightBlinker();
                        break;
                    }
            }
        }
    }

    public void Dispose() => _dInputController?.Dispose();


    internal enum InputType
    {
        None,
        LeftOn, RightOn,
        LeftOff, RightOff
    }
}
