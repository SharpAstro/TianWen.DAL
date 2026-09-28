using System;
using System.Collections.Generic;
using System.Threading;

namespace TianWen.DAL
{
    /// <summary>
    /// One native session per device, shared by every holder: the first <see cref="Acquire"/> of a key opens it, the last
    /// <see cref="Release"/> closes it. The promise <see cref="INativeDeviceInfo.Open"/> and <see cref="INativeDeviceInfo.Close"/>
    /// make, stated once for every binding.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why a binding needs it.</b> A vendor SDK has one session per device however many times it is opened, and every
    /// enumeration opens a device to read its serial and closes it again, as does a connect comparing serials. Uncounted, a
    /// listing closes the camera a driver is using: an ASI462MC streaming at 92 frames a second failed
    /// <c>ASI_ERROR_CAMERA_CLOSED</c> 72 ms after one (2026-09-28). The QHYCCD and ToupTek bindings each counted opens in a
    /// registry of their own; the ZWO and Player One bindings did not.
    /// </para>
    /// <para>
    /// <typeparamref name="TSession"/> is what the session carries (a native handle, the state a callback writes), for an SDK
    /// that addresses a device through one; <see cref="SharedSessions{TKey}"/> is the form for an SDK addressed by an id alone.
    /// </para>
    /// </remarks>
    public sealed class SharedSessions<TKey, TSession> where TKey : notnull where TSession : class
    {
        // Why a lock: the count and the native open or close must move together, exactly once per transition of the count,
        // and a compare-and-swap on a dictionary cannot hold a native call inside it. Opens and closes are connects,
        // disconnects and enumerations, never a frame path, so it is uncontended in practice.
        private readonly Lock _lock = new Lock();
        private readonly Dictionary<TKey, (TSession Session, int Holders)> _sessions = new Dictionary<TKey, (TSession Session, int Holders)>();

        /// <summary>
        /// The session for <paramref name="key"/>, opened by <paramref name="open"/> for the first holder and shared by every
        /// later one; null when the first open fails.
        /// </summary>
        public TSession? Acquire(TKey key, Func<TKey, TSession?> open)
        {
            lock (_lock)
            {
                if (_sessions.TryGetValue(key, out var held))
                {
                    _sessions[key] = (held.Session, held.Holders + 1);
                    return held.Session;
                }

                if (open(key) is not { } session)
                {
                    return null;
                }

                _sessions[key] = (session, 1);
                return session;
            }
        }

        /// <summary>
        /// Lets go of one hold on <paramref name="key"/>, calling <paramref name="close"/> for the last. A key no holder
        /// acquired here goes to <paramref name="close"/> with no session, as a bare close would have.
        /// </summary>
        public bool Release(TKey key, Func<TKey, TSession?, bool> close)
        {
            lock (_lock)
            {
                if (!_sessions.TryGetValue(key, out var held))
                {
                    return close(key, null);
                }

                if (held.Holders > 1)
                {
                    _sessions[key] = (held.Session, held.Holders - 1);
                    return true;
                }

                _sessions.Remove(key);
                return close(key, held.Session);
            }
        }

        /// <summary>The session open for <paramref name="key"/>, or null when no one holds it.</summary>
        public TSession? Find(TKey key)
        {
            lock (_lock)
            {
                return _sessions.TryGetValue(key, out var held) ? held.Session : null;
            }
        }

        /// <summary>How many holders have <paramref name="key"/> open.</summary>
        public int HoldersOf(TKey key)
        {
            lock (_lock)
            {
                return _sessions.TryGetValue(key, out var held) ? held.Holders : 0;
            }
        }
    }

    /// <summary>
    /// <see cref="SharedSessions{TKey, TSession}"/> for an SDK that addresses a device by its id alone (ZWO, Player One), so a
    /// session carries nothing but the fact that it is open.
    /// </summary>
    public sealed class SharedSessions<TKey> where TKey : notnull
    {
        private static readonly object IsOpen = new object();

        private readonly SharedSessions<TKey, object> _sessions = new SharedSessions<TKey, object>();

        /// <summary>Opens <paramref name="key"/> for one more holder, calling <paramref name="open"/> only for the first.</summary>
        public bool Acquire(TKey key, Func<TKey, bool> open) => _sessions.Acquire(key, k => open(k) ? IsOpen : null) is not null;

        /// <summary>Closes <paramref name="key"/> for one holder, calling <paramref name="close"/> only for the last.</summary>
        public bool Release(TKey key, Func<TKey, bool> close) => _sessions.Release(key, (k, _) => close(k));

        /// <summary>How many holders have <paramref name="key"/> open.</summary>
        public int HoldersOf(TKey key) => _sessions.HoldersOf(key);
    }
}
