using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace ToraConHelper.Services;

public class GameProcessDetector : IDisposable
{
    private static readonly string[] targets = new string[] { "eurotrucks2.exe", "amtrucks.exe" };


    private bool _started;

    //private List<ManagementEventWatcher> startEvents = new(targets.Length);
    private List<ManagementEventWatcher> endEvents = new(targets.Length);

    //public event EventHandler<EventArgs>? GameProcessStarted;
    public event EventHandler<EventArgs>? GameProcessEnded;

    public GameProcessDetector() { }

    public bool IsStarted { get { return _started; } }

    public void StartWatchers()
    {
        StopWatchers();
        foreach (var target in targets)
        {
            //startEvents.Add(WatchForProcessStart(target));
            endEvents.Add(WatchForProcessEnd(target));
        }
        // check already started
        //if (targets.Any(t => Process.GetProcessesByName(Path.GetFileNameWithoutExtension(t)).Any()))
        //{
        //    GameProcessStarted?.Invoke(this, EventArgs.Empty);
        //}
        _started = true;
    }

    public void StopWatchers()
    {
        //foreach (var se in startEvents)
        //{
        //    se.EventArrived -= ProcessStarted;
        //    se.Stop();
        //    se.Dispose();
        //}
        foreach (var ee in endEvents)
        {
            ee.EventArrived += ProcessEnded;
            ee.Stop();
            ee.Dispose();
        }
        //startEvents.Clear();
        endEvents.Clear();
        _started = false;
    }

    //private ManagementEventWatcher WatchForProcessStart(string processName)
    //{
    //    string queryString =
    //        "SELECT TargetInstance" +
    //        "  FROM __InstanceCreationEvent " +
    //        "WITHIN  10 " +
    //        " WHERE TargetInstance ISA 'Win32_Process' " +
    //        "   AND TargetInstance.Name = '" + processName + "'";

    //    // The dot in the scope means use the current machine
    //    string scope = @"\\.\root\CIMV2";

    //    // Create a watcher and listen for events
    //    ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
    //    watcher.EventArrived += ProcessStarted;
    //    watcher.Start();
    //    return watcher;
    //}

    private ManagementEventWatcher WatchForProcessEnd(string processName)
    {
        string queryString =
            "SELECT TargetInstance" +
            "  FROM __InstanceDeletionEvent " +
            "WITHIN  10 " +
            " WHERE TargetInstance ISA 'Win32_Process' " +
            "   AND TargetInstance.Name = '" + processName + "'";

        // The dot in the scope means use the current machine
        string scope = @"\\.\root\CIMV2";

        // Create a watcher and listen for events
        ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
        watcher.EventArrived += ProcessEnded;
        watcher.Start();
        return watcher;
    }

    private void ProcessEnded(object sender, EventArrivedEventArgs e)
    {
        Debug.WriteLine($"{nameof(ProcessEnded)}");
        GameProcessEnded?.Invoke(this, e);
    }

    //private void ProcessStarted(object sender, EventArrivedEventArgs e)
    //{
    //    Debug.WriteLine($"{nameof(ProcessStarted)}");
    //    GameProcessStarted?.Invoke(this, e);
    //}

    public void Dispose()
    {
        if (_started) StopWatchers();
    }
}
