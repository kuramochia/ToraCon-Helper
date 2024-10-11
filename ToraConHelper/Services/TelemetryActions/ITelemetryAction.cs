using SCSSdkClient.Object;
using System;

namespace ToraConHelper.Services.TelemetryActions;

public interface ITelemetryAction
{
    /// <summary>
    /// テレメトリが更新されると呼び出されます。
    /// </summary>
    /// <param name="telemetry"></param>
    void OnTelemetryUpdated(SCSTelemetry telemetry);
    void OnActionAdded();
    void OnActionRemoved();
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

public abstract class TelemetryActionBase : ITelemetryAction
{
    public abstract void OnTelemetryUpdated(SCSTelemetry telemetry);

    public virtual void OnActionAdded() { }
    public virtual void OnActionRemoved() { }
}

public abstract class TelemetryActionWithEventsBase : TelemetryActionBase, ITelemetryActionWithEvents
{
    public virtual void OnFerry() { }

    public virtual void OnFined() { }

    public virtual void OnJobCancelled() { }

    public virtual void OnJobDelivered() { }

    public virtual void OnJobStarted() { }

    public virtual void OnRefuelEnd() { }

    public virtual void OnRefuelPayed() { }

    public virtual void OnRefuelStart() { }

    public virtual void OnTollgate() { }

    public virtual void OnTrain() { }
}
