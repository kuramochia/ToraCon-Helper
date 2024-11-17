using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vortice.DirectInput;

namespace ToraConHelper.Services;

/// <summary>
/// DirectInput コントロール
/// </summary>
public class DirectInputController : IDisposable
{
    // トラコンの ProductGUID
    private static readonly Guid TrackControlSystemProductGuid = new("{017a0f0d-0000-0000-0000-504944564944}");
    public IDirectInputDevice8? Device { get; private set; }

    public bool IsInitialized { get { return Device != null; } }
    public bool Initialize()
    {
        if (Device != null) return true;
        try
        {
            using IDirectInput8 dinput = DInput.DirectInput8Create();
            // デバイスタイプは Driving 固定
            IList<DeviceInstance> devices = dinput.GetDevices(DeviceType.Driving, DeviceEnumerationFlags.AttachedOnly);

            // トラコンの GUID で一本釣り
            DeviceInstance deviceInstance = devices.FirstOrDefault(d => d.ProductGuid == TrackControlSystemProductGuid);
            if (deviceInstance == null) return false;

            IDirectInputDevice8 device = dinput.CreateDevice(deviceInstance.InstanceGuid);

            device.SetCooperativeLevel((IntPtr)device, CooperativeLevel.NonExclusive | CooperativeLevel.Background);
            device.SetDataFormat<RawJoystickState>();
            device.Properties.BufferSize = 1024;
            device.Acquire();
            Device = device;
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Release();
            return false;
        }
    }

    /// <summary>
    /// バッファされているジョイスティック情報を取得
    /// </summary>
    /// <returns></returns>
    public JoystickUpdate[]? GetBufferedJoystickData()
    {
        try
        {
            return Device?.GetBufferedJoystickData();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Release();
            return null;
        }
    }

    public void Release()
    {
        try
        {
            if (Device != null)
            {
                Device.Unacquire();
                Device.Release();
                Device.Dispose();
                Device = null;
            }
        }
        catch { }
    }

    public void Dispose() => Release();
}
