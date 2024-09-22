using SCSSdkClient.Input;
using SCSSdkClient.Object;
using System.Diagnostics;

namespace ToraConHelper.Services.TelemetryActions;

internal class AutoFullfuelAction : ITelemetryActionWithEvents
{
    public void OnTelemetryUpdated(SCSTelemetry telemetry)
    {
    }

    public void OnRefuelStart()
    {
        //Debug.WriteLine(nameof(OnRefuelStart));
        SetActivate(true);
    }

    public void OnRefuelEnd()
    {
        //Debug.WriteLine(nameof(OnRefuelEnd));
        SetActivate(false);
    }

    public void OnRefuelPayed()
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


    #region Unused events
    public void OnFerry() { }

    public void OnFined() { }

    public void OnJobCancelled() { }

    public void OnJobDelivered() { }

    public void OnJobStarted() { }
    public void OnTollgate() { }

    public void OnTrain() { }
    #endregion
}
