using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

public class ReterderSkipInputAction : TelemetryActionBase
{
    // スキップ レベル
    public int SkipLevel { get; set; } = 1;

    private uint _reterderLevel = 0;
    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var changed = false;
        var reterderLevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
        var stepCount = telemetry.TruckValues.ConstantsValues.MotorValues.RetarderStepCount;

        // リターダーUp
        if (reterderLevel > _reterderLevel)
        {
            Debug.WriteLine($"Up, Current Reterder:{reterderLevel}, StepCount:{stepCount}");

            // SkipLevel 分入力
            uint nextLevel = (uint)Math.Min(reterderLevel + SkipLevel, stepCount);
            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(nextLevel);
            _reterderLevel = nextLevel;
            changed = true;
            Debug.WriteLine($"Skip Input Current:{_reterderLevel}");
        }
        // リターダーDown
        else if (reterderLevel < _reterderLevel)
        {
            Debug.WriteLine($"Down, Current Reterder:{reterderLevel}, StepCount:{stepCount}");

            // SkipLevel 分入力
            uint nextLevel = (uint)Math.Max(reterderLevel - SkipLevel, 0);

            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(nextLevel);
            _reterderLevel = nextLevel;
            changed = true;
            Debug.WriteLine($"Skip Input Current:{_reterderLevel}");
        }
        else
        {
            _reterderLevel = reterderLevel;
        }

        return changed;
    }
}
