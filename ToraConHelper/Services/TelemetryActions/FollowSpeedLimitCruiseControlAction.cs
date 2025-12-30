using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

public class FollowSpeedLimitCruiseControlAction : TelemetryActionBase
{
    public int CruiseControlStep { get; set; } = 5;
    public bool MphInATS { get; set; } = true;

    // CCが有効かどうか
    private bool ccEnabled = false;

    // 前回CC変更があったかどうか
    private bool ccLastChanged = false;

    // 前回のCC速度
    private double lastCCSpeed = 0d;

    // ユーザーがCC速度を変更したかどうか
    // 一度変更したら、CCがオフになるまで追従を止める
    private bool isUserChanged = false;

    public override bool OnTelemetryUpdated(SCSTelemetry telemetry)
    {
        bool changed = false;
        if (telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControl)
        {
            var useMph = MphInATS && telemetry.Game == SCSSdkClient.SCSGame.Ats;

            var currentCc = Math.Floor(useMph ? telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControlSpeed.Mph : telemetry.TruckValues.CurrentValues.DashboardValues.CruiseControlSpeed.Kph);

            // ユーザーが操作して CC速度を変更した場合は、追従を止める
            // CCが有効で、前回変更が無く、前回CC速度と現在CC速度が異なり、前回CC速度が0でない場合
            if (ccEnabled && !ccLastChanged && lastCCSpeed != currentCc && lastCCSpeed != 0d)
            {
                isUserChanged = true;
                Debug.WriteLine("User changed CC speed. Stop following.");
            }

            if (!isUserChanged)
            {
                // Off から On になった場合、初回として扱う
                var firstTime = false;
                if (!ccEnabled) firstTime = true;
                ccEnabled = true;

                var currentLimit = Math.Floor(useMph ? telemetry.NavigationValues.SpeedLimit.Mph : telemetry.NavigationValues.SpeedLimit.Kph);

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
                lastCCSpeed = currentCc;
            }
        }
        else
        {
            ccEnabled = false;
            isUserChanged = false;
            lastCCSpeed = 0d;
        }
        ccLastChanged = changed;
        return changed;
    }

    // 優先度：低（WoT で制限速度がある場合に更新し続けるため）
    public override TelemetryActionPriority Priority => TelemetryActionPriority.Low;
}