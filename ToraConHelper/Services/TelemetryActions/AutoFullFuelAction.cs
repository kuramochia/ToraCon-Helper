using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

internal class AutoFullfuelAction : TelemetryActionWithEventsBase
{
    public override bool OnTelemetryUpdated(SCSTelemetry telemetry) => false;


    public override void OnRefuelStart()
    {
        //Debug.WriteLine(nameof(OnRefuelStart));
        SetActivate(true);
    }

    public override void OnRefuelEnd()
    {
        //Debug.WriteLine(nameof(OnRefuelEnd));
        SetActivate(false);
    }

    public override void OnRefuelPayed()
    {
        //Debug.WriteLine(nameof(OnRefuelPayed));
        SetActivate(false);
    }

    private void SetActivate(bool value)
    {
        //Debug.WriteLine($"{nameof(SetActivate)}({value})");
        using var input = new SCSSdkTelemetryInput();
        input.Connect();
        input.SetActivate(value);
    }

}
