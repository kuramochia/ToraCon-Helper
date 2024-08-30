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

    private readonly List<ITelemetryAction> _actions = new();

    public TelemetryActionsManager() { }

    public ReadOnlyCollection<ITelemetryAction> Actions { get { return _actions.AsReadOnly(); } }

    public void AddAction(ITelemetryAction action) => _actions.Add(action);

    public void RemoveAction(ITelemetryAction action) => _actions.Remove(action);

    public void Start()
    {
        Stop();
        _telemetry = new SCSSdkTelemetry();

        _telemetry.Data += Telemetry_Data;
        Debug.WriteLine($"SCSSdkTelemetry Start. UpdateInterval={_telemetry.UpdateInterval}ms");
    }

    private bool _running = false;

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
    }
}
