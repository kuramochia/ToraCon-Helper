using SCSSdkClient.Object;

namespace ToraConHelper.Services.TelemetryActions;

public interface ITelemetryAction
{
    /// <summary>
    /// テレメトリが更新されると呼び出されます。
    /// </summary>
    /// <param name="telemetry"></param>
    public void OnTelemetryUpdated(SCSTelemetry telemetry);
}
