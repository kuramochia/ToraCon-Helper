using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.Services;

public class TelemetryActionsManager : IDisposable
{
    private SCSSdkTelemetry? _telemetry;

    private bool _running = false;

    private readonly List<ITelemetryAction> _actions = [];

    private readonly List<ITelemetryActionWithEvents> _actionsWithEvents = [];

    public TelemetryActionsManager() { }

    public ReadOnlyCollection<ITelemetryAction> Actions { get { return _actions.AsReadOnly(); } }

    public void AddAction(ITelemetryAction action)
    {
        _actions.Add(action);
        if (action is ITelemetryActionWithEvents actionWithEvents) _actionsWithEvents.Add(actionWithEvents);
        action.OnActionAdded();
    }

    public void RemoveAction(ITelemetryAction action)
    {
        _actions.Remove(action);
        if (action is ITelemetryActionWithEvents actionWithEvents) _actionsWithEvents.Remove(actionWithEvents);
        action.OnActionRemoved();
    }

    public void Start()
    {
        Stop();
        _telemetry = new SCSSdkTelemetry();
        _telemetry.Data += Telemetry_Data;
        _telemetry.JobStarted += Telemetry_JobStarted;
        _telemetry.JobCancelled += Telemetry_JobCancelled;
        _telemetry.Fined += Telemetry_Fined;
        _telemetry.Tollgate += Telemetry_Tollgate;
        _telemetry.Ferry += Telemetry_Ferry;
        _telemetry.Train += Telemetry_Train;
        _telemetry.RefuelStart += Telemetry_RefuelStart;
        _telemetry.RefuelEnd += Telemetry_RefuelEnd;
        _telemetry.RefuelPayed += Telemetry_RefuelPayed;
        Debug.WriteLine($"SCSSdkTelemetry Start. UpdateInterval={_telemetry.UpdateInterval}ms");
    }

    #region event handler

    private void Telemetry_RefuelPayed(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnRefuelPayed();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    private void Telemetry_RefuelEnd(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnRefuelEnd();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    private void Telemetry_RefuelStart(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnRefuelStart();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    private void Telemetry_Train(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnTrain();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    private void Telemetry_Ferry(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnFerry();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    private void Telemetry_Tollgate(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnTollgate();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    private void Telemetry_Fined(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnFined();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    private void Telemetry_JobCancelled(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnJobCancelled();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    private void Telemetry_JobStarted(object sender, EventArgs e)
    {
        if (_running) return;
        try
        {
            _running = true;
            foreach (var act in _actionsWithEvents)
            {
                act?.OnJobStarted();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    #endregion

    private void Telemetry_Data(SCSTelemetry data, bool updated)
    {
        if (!updated || _running || !data.SdkActive) return;
        try
        {
            _running = true;
            foreach (var act in _actions)
            {
                if (act != null && act.OnTelemetryUpdated(data)) break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Telemetry was closed: {ex}");
        }
        finally
        {
            _running = false;
        }
    }

    public void Stop()
    {
        if (_telemetry != null)
        {
            _telemetry.Data -= Telemetry_Data;
            _telemetry.JobStarted -= Telemetry_JobStarted;
            _telemetry.JobCancelled -= Telemetry_JobCancelled;
            _telemetry.Fined -= Telemetry_Fined;
            _telemetry.Tollgate -= Telemetry_Tollgate;
            _telemetry.Ferry -= Telemetry_Ferry;
            _telemetry.Train -= Telemetry_Train;
            _telemetry.RefuelStart -= Telemetry_RefuelStart;
            _telemetry.RefuelEnd -= Telemetry_RefuelEnd;
            _telemetry.RefuelPayed -= Telemetry_RefuelPayed;
            _telemetry.Dispose();
            _telemetry = null;
        }
    }

    public void Dispose()
    {
        Stop();
        foreach (var act in _actions)
            (act as IDisposable)?.Dispose();
        _actions.Clear();
        _actionsWithEvents.Clear();
    }
}
