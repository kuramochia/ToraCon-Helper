using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ToraConHelper.Helpers;
using Vortice.DirectInput;

namespace ToraConHelper.Services;

/// <summary>
/// DirectInput コントロール
/// </summary>
public class DirectInputController : IDisposable
{
    // トラコンの ProductGUID
    private static readonly Guid TrackControlSystemProductGuid = new("{017a0f0d-0000-0000-0000-504944564944}");

    private IDirectInputDevice8? _device;
    private IDirectInput8? _dinput;

    private readonly LockOutHelper _lockOutHelper = new(TimeSpan.FromSeconds(15));

    public bool IsInitialized { get { return _dinput != null && _device != null; } }
    public bool Initialize()
    {
        // ロックアウト中なら false
        if (_lockOutHelper.IsLockedOut)
        {
            return false;
        }

        if (IsInitialized)
        {
            _lockOutHelper.ResetLockout();
            return true;
        }

        var result = InitializeCore();

        if (result)
        {
            _lockOutHelper.ResetLockout();
        }
        else
        {
            // トラコン未接続時の CPU 負荷を下げるため、1回失敗で15秒ロックアウトする
            // トラコンが接続されていない状態でゲームスタートすると、ほぼ15秒に一回、DInput の初期化を試みることになる
            _lockOutHelper.LockOut();
            Debug.WriteLine("DirectInputController: Locked out until " + _lockOutHelper.RemainingLockout);
        }

        return result;
    }

    private bool InitializeCore()
    {
        try
        {
            if (_dinput == null)
            {
                _dinput = DInput.DirectInput8Create();
            }
            // デバイスタイプは Driving 固定
            IList<DeviceInstance> devices = _dinput.GetDevices(DeviceType.Driving, DeviceEnumerationFlags.AttachedOnly);

            // トラコンの GUID で一本釣り
            DeviceInstance deviceInstance = devices.FirstOrDefault(d => d.ProductGuid == TrackControlSystemProductGuid);
            if (deviceInstance == null) return false;

            _device = _dinput.CreateDevice(deviceInstance.InstanceGuid);

            _device.SetCooperativeLevel((IntPtr)_device, CooperativeLevel.NonExclusive | CooperativeLevel.Background);
            _device.SetDataFormat<RawJoystickState>();
            _device.Properties.BufferSize = 1024;
            _device.Acquire();
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
            return _device?.GetBufferedJoystickData();
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
            _device?.Unacquire();
            _device?.Dispose();
            _dinput?.Dispose();
        }
        catch { }
        finally
        {
            _device = null;
            _dinput = null;
        }
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }
}
