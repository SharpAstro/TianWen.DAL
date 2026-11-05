using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TianWen.DAL;

/// <summary>
/// A short blocking wait that ends when it should: for a vendor SDK whose frame call does not wait, so the caller polls.
/// </summary>
/// <remarks>
/// <para><b>On Windows <see cref="Thread.Sleep(TimeSpan)"/> of a millisecond lasts a whole tick of the system timer, 15.625 ms by
/// default</b>, and nothing in TianWen calls <c>timeBeginPeriod(1)</c>, which would change the timer for the whole process.
/// Polled that way, QHY's <c>GetQHYCCDLiveFrame</c> was asked about every 15 ms (bench, 2026-11-05, a QHY PoleMaster), so each
/// frame was picked up as much as a tick late. Here the wait is a high-resolution waitable timer
/// (<c>CREATE_WAITABLE_TIMER_HIGH_RESOLUTION</c>, Windows 10 1803 and later), which ends this one wait on time and leaves the
/// rest of the process as it was; the thread still sleeps, it does not spin. Where that timer cannot be made, and on every other
/// system, whose sleeps are already fine-grained, it is <see cref="Thread.Sleep(TimeSpan)"/>.</para>
/// <para>A blocking wait, for a vendor SDK's own reader thread (the DAL's <c>GetVideoData</c> blocks by contract). An async loop
/// in a consumer waits on its clock instead, so a fake clock can drive it.</para>
/// </remarks>
public static partial class PreciseWait
{
    private const uint CreateWaitableTimerHighResolution = 0x2;
    private const uint TimerAllAccess = 0x1F0003;
    private const uint Infinite = 0xFFFFFFFF;

    /// <summary>
    /// Blocks the calling thread for <paramref name="duration"/>, ending within about a millisecond of it on Windows rather than
    /// at the next timer tick. Zero or less returns at once.
    /// </summary>
    public static void Sleep(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            return;
        }

        if (!OperatingSystem.IsWindows() || !TrySleepHighResolution(duration))
        {
            Thread.Sleep(duration);
        }
    }

    /// <summary>One timer per wait, made and closed around it: cheap next to the wait itself, and nothing to leak per thread.</summary>
    private static bool TrySleepHighResolution(TimeSpan duration)
    {
        var timer = CreateWaitableTimerExW(IntPtr.Zero, IntPtr.Zero, CreateWaitableTimerHighResolution, TimerAllAccess);
        if (timer == IntPtr.Zero)
        {
            // Older Windows refuses the flag: the caller falls back to an ordinary sleep.
            return false;
        }

        try
        {
            // Negative is relative, in 100 ns units.
            var dueTime = -Math.Max(1L, duration.Ticks);
            return SetWaitableTimer(timer, in dueTime, 0, IntPtr.Zero, IntPtr.Zero, false)
                && WaitForSingleObject(timer, Infinite) == 0;
        }
        finally
        {
            CloseHandle(timer);
        }
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr CreateWaitableTimerExW(IntPtr attributes, IntPtr name, uint flags, uint desiredAccess);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWaitableTimer(IntPtr timer, in long dueTime, int period, IntPtr completionRoutine, IntPtr argument,
        [MarshalAs(UnmanagedType.Bool)] bool resume);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial uint WaitForSingleObject(IntPtr handle, uint milliseconds);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(IntPtr handle);
}
