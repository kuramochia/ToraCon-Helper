using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

/// <summary>
/// リバース時に自動的にフラッシャーを点灯させるアクション
/// </summary>
internal class AutoFlasherAtReverseAction : TelemetryActionBase
{
    // リバース解除した時にフラッシャーが点灯していても無視するかどうか
    public bool IgnoreFlasherOffWhenReverseOff { get; set; } = false;

    // 以前のリバースの値
    private bool _isReversing = false;
    // リバースしている状態に変化した時に、フラッシャーが既に点灯している場合のフラグ。これが true の場合、リバース、リバース解除の一連の流れまではフラッシャーの状態を変更しない。
    private bool _isFlasherOnWhenReverseStarted = false;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        var isFlasherOn = telemetry.TruckValues.CurrentValues.LightsValues.HazardWarningLights;
        var isReversing = telemetry.TruckValues.CurrentValues.LightsValues.Reverse;

        var changed = false;

        // リバース開始したタイミングで、フラッシャーが点灯している場合は、リバース状態の変化に伴うフラッシャーのオンオフを無視する
        if (!_isReversing && isReversing && isFlasherOn)
        {
            _isFlasherOnWhenReverseStarted = true;
        }

        // リバースしていない状態からリバースに切り替わり、フラッシャーが消灯している場合、フラッシャーを点灯させる
        if (!_isFlasherOnWhenReverseStarted && !_isReversing && isReversing && !isFlasherOn)
        {
            Debug.WriteLine("AutoFlasherAtReverseAction: Reversing started, turning on flasher.");
            PushToggleFlasherInput();
            changed = true;
        }
        // リバースしている状態からリバースを解除し、フラッシャーが点灯している場合、フラッシャーを消灯させる
        else if (!_isFlasherOnWhenReverseStarted && _isReversing && !isReversing && isFlasherOn && !IgnoreFlasherOffWhenReverseOff)
        {
            Debug.WriteLine("AutoFlasherAtReverseAction: Reversing stopped, turning off flasher.");
            PushToggleFlasherInput();
            changed = true;
        }

        // リバース解除されたら、状態をリセット
        if (_isReversing && !isReversing)
        {
            _isFlasherOnWhenReverseStarted = false;
        }

        _isReversing = isReversing;
        return changed;
    }

    private static void PushToggleFlasherInput()
    {
        using var input = new SCSSdkTelemetryInput();
        input.Connect();
        input.SetFlasher4way();
    }
}
