using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

public class RetarderSkipInputAction : TelemetryActionBase
{
    // スキップ レベル
    public int SkipLevel { get; set; } = 1;

    private uint _retarderLevel = 0;
    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var changed = false;
        var retarderLevel = telemetry.TruckValues.CurrentValues.MotorValues.BrakeValues.RetarderLevel;
        var stepCount = telemetry.TruckValues.ConstantsValues.MotorValues.RetarderStepCount;

        // リターダーUp
        if (retarderLevel > _retarderLevel)
        {
            Debug.WriteLine($"Up, Current Retarder:{retarderLevel}, StepCount:{stepCount}");

            // SkipLevel 分入力
            uint nextLevel = (uint)Math.Min(retarderLevel + SkipLevel, stepCount);
            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(nextLevel);
            _retarderLevel = nextLevel;
            changed = true;
            Debug.WriteLine($"Skip Input Current:{_retarderLevel}");
        }
        // リターダーDown
        else if (retarderLevel < _retarderLevel)
        {
            Debug.WriteLine($"Down, Current Retarder:{retarderLevel}, StepCount:{stepCount}");

            // SkipLevel 分入力
            uint nextLevel = (uint)Math.Max(retarderLevel - SkipLevel, 0);

            using var input = new SCSSdkTelemetryInput();
            input.Connect();
            input.SetRetarder(nextLevel);
            _retarderLevel = nextLevel;
            changed = true;
            Debug.WriteLine($"Skip Input Current:{_retarderLevel}");
        }
        else
        {
            _retarderLevel = retarderLevel;
        }

        return changed;
    }
}
