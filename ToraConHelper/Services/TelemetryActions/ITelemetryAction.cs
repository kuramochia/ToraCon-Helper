using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public interface ITelemetryAction
{
    /// <summary>
    /// テレメトリが更新されると呼び出されます。
    /// </summary>
    /// <param name="telemetry"></param>
    void OnTelemetryUpdated(SCSTelemetry telemetry);
}

/// <summary>
/// テレメトリの特殊イベントが発行されると呼び出されます。
/// </summary>
public interface ITelemetryActionWithEvents : ITelemetryAction
{
    void OnJobStarted();
    void OnJobCancelled();
    void OnJobDelivered();
    void OnFined();
    void OnTollgate();
    void OnFerry();
    void OnTrain();
    void OnRefuelStart();
    void OnRefuelEnd();
    void OnRefuelPayed();
}
