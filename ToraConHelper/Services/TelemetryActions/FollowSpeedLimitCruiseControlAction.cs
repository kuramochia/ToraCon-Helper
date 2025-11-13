using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

public class FollowSpeedLimitCruiseControlAction : TelemetryActionBase
{
    public int CruiseControlStep { get; set; } = 5;
    public bool MphInATS { get; set; } = true;

    private bool ccEnabled = false;
    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        bool changed = false;
        if (telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControl)
        {
            // Off から On になった場合、初回として扱う
            var firstTime = false;
            if (!ccEnabled) firstTime = true;
            ccEnabled = true;

            var useMph = MphInATS && telemetry.Game == SCSSdkClient.SCSGame.Ats;

            var currentLimit = Math.Floor(useMph ? telemetry.NavigationValues.SpeedLimit.Mph : telemetry.NavigationValues.SpeedLimit.Kph);
            var currentCc = Math.Floor(useMph ? telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControlSpeed.Mph : telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControlSpeed.Kph);

            Debug.WriteLine($"Limit={currentLimit} CC={currentCc} UseMph={useMph}");

            if (currentLimit > 0 && currentCc > 0)
            {
                // 制限速度 > CC速度 + ステップ か、初回で 制限速度 > CC速度 の場合、CC速度を上げる
                if ((currentLimit >= currentCc + CruiseControlStep) || (firstTime && currentLimit > currentCc))
                {
                    //CC速度UP
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetCruiseControlIncrement(30);
                    changed = true;
                    Debug.WriteLine("CC Increment");
                }
                else if (currentLimit < currentCc)
                {
                    //CC速度Down
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetCruiseControlDecrement(30);
                    changed = true;
                    Debug.WriteLine("CC Decrement");
                }
            }
        }
        else
        {
            ccEnabled = false;
        }

        return changed;
    }

    // 優先度：低（WoT で制限速度がある場合に更新し続けるため）
    public override TelemetryActionPriority Priority => TelemetryActionPriority.Low;
}