using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

public class FollowSpeedLimitCruiseControlAction : TelemetryActionBase
{
    public int CruiseControlStep { get; set; } = 5;
    public bool MphInATS { get; set; } = true;
    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        bool changed = false;
        if (telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControl)
        {
            var useMph = MphInATS && telemetry.Game == SCSSdkClient.SCSGame.Ats;

            var currentLimit = Math.Floor(useMph ? telemetry.NavigationValues.SpeedLimit.Mph : telemetry.NavigationValues.SpeedLimit.Kph);
            var currentCc = Math.Floor(useMph ? telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControlSpeed.Mph : telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControlSpeed.Kph);

            Debug.WriteLine($"Limit={currentLimit} CC={currentCc} UseMph={useMph}");

            if (currentLimit > 0 && currentCc > 0)
            {
                // 制限速度 > CC速度 + ステップ
                if (currentLimit >= currentCc + CruiseControlStep)
                {
                    //CC速度UP
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetCruiseControlIncrement();
                    changed = true;
                    Debug.WriteLine("CC Increment");
                }
                else if (currentLimit < currentCc)
                {
                    //CC速度Down
                    using var input = new SCSSdkTelemetryInput();
                    input.Connect();
                    input.SetCruiseControlDecrement();
                    changed = true;
                    Debug.WriteLine("CC Decrement");
                }
            }
        }
        return changed;
    }
}