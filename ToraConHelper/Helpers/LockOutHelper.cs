using System;

namespace ToraConHelper.Helpers;

/// <summary>
/// 連続で呼び出される場合に CPU 使用率が高くなるのを防ぐため、一定時間はロックアウト処理を行うヘルパークラス。
/// </summary>
internal class LockOutHelper
{
    private DateTime? lockoutUntil;
    internal LockOutHelper(TimeSpan lockoutDuration)
    {
        LockoutDuration = lockoutDuration;
    }

    internal TimeSpan LockoutDuration { get; }
    internal TimeSpan RemainingLockout => lockoutUntil.HasValue ? lockoutUntil.Value - DateTime.Now : TimeSpan.Zero;

    internal bool IsLockedOut => lockoutUntil.HasValue && DateTime.Now < lockoutUntil.Value;

    internal void LockOut() => lockoutUntil = DateTime.Now.Add(LockoutDuration);

    internal void ResetLockout() => lockoutUntil = null;
}
