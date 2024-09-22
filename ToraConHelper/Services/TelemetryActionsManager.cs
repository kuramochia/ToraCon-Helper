using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Markup;
using ToraConHelper.Services.TelemetryActions;

namespace ToraConHelper.Services;

public class TelemetryActionsManager : IDisposable
{
    private SCSSdkTelemetry? _telemetry;

    private bool _running = false;

    private readonly List<ITelemetryAction> _actions = new();

    private readonly List<ITelemetryActionWithEvents> _actionsWithEvents = new();

    public TelemetryActionsManager() { }

    public ReadOnlyCollection<ITelemetryAction> Actions { get { return _actions.AsReadOnly(); } }

    public void AddAction(ITelemetryAction action)
    {
        _actions.Add(action);
        if(action is ITelemetryActionWithEvents actionWithEvents) _actionsWithEvents.Add(actionWithEvents);
    }

    public void RemoveAction(ITelemetryAction action)
    {
        _actions.Remove(action);
        if (action is ITelemetryActionWithEvents actionWithEvents) _actionsWithEvents.Remove(actionWithEvents);
    }

    public void Start()
    {
        Stop();
        _telemetry = new SCSSdkTelemetry();
        _telemetry.Data += Telemetry_Data;
        _telemetry.JobStarted += _telemetry_JobStarted;
        _telemetry.JobCancelled += _telemetry_JobCancelled;
        _telemetry.Fined += _telemetry_Fined;
        _telemetry.Tollgate += _telemetry_Tollgate;
        _telemetry.Ferry += _telemetry_Ferry;
        _telemetry.Train += _telemetry_Train;
        _telemetry.RefuelStart += _telemetry_RefuelStart;
        _telemetry.RefuelEnd += _telemetry_RefuelEnd;
        _telemetry.RefuelPayed += _telemetry_RefuelPayed;
        Debug.WriteLine($"SCSSdkTelemetry Start. UpdateInterval={_telemetry.UpdateInterval}ms");
    }

    #region event handler

    private void _telemetry_RefuelPayed(object sender, EventArgs e)
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

    private void _telemetry_RefuelEnd(object sender, EventArgs e)
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

    private void _telemetry_RefuelStart(object sender, EventArgs e)
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

    private void _telemetry_Train(object sender, EventArgs e)
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

    private void _telemetry_Ferry(object sender, EventArgs e)
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

    private void _telemetry_Tollgate(object sender, EventArgs e)
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

    private void _telemetry_Fined(object sender, EventArgs e)
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

    private void _telemetry_JobCancelled(object sender, EventArgs e)
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

    private void _telemetry_JobStarted(object sender, EventArgs e)
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
        if (!updated || _running) return;
        try
        {
            _running = true;
            foreach (var act in _actions)
            {
                act?.OnTelemetryUpdated(data);
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
            _telemetry.JobStarted -= _telemetry_JobStarted;
            _telemetry.JobCancelled -= _telemetry_JobCancelled;
            _telemetry.Fined -= _telemetry_Fined;
            _telemetry.Tollgate -= _telemetry_Tollgate;
            _telemetry.Ferry -= _telemetry_Ferry;
            _telemetry.Train -= _telemetry_Train;
            _telemetry.RefuelStart -= _telemetry_RefuelStart;
            _telemetry.RefuelEnd -= _telemetry_RefuelEnd;
            _telemetry.RefuelPayed -= _telemetry_RefuelPayed;
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
