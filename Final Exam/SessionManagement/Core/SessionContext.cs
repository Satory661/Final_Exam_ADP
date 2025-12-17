using System;
using System.Threading;
using SessionManagement.Models;
using SessionManagement.Interfaces;
using SessionManagement.Stores;
using SessionManagement.Cleaners;

namespace SessionManagement.Core
{
    public sealed class SessionContext : ISessionContext, IDisposable
    {
        private static readonly Lazy<SessionContext> lazy =
            new Lazy<SessionContext>(() => new SessionContext(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static SessionContext Instance => lazy.Value;

        private readonly ISessionStore store;
        private readonly ISessionCleaner cleaner;

        private Timer timer;
        private TimeSpan interval = Timeout.InfiniteTimeSpan;
        private TimeSpan timeout = TimeSpan.FromMinutes(30);
        private int cleaning = 0;

        private SessionContext()
        {
            store = new InMemorySessionStore();
            cleaner = new InactivitySessionCleaner();
        }
        
        public void ConfigureCleanup(TimeSpan runInterval, TimeSpan sessionTimeout)
        {
            interval = runInterval;
            timeout = sessionTimeout;

            timer?.Dispose();
            if (runInterval == Timeout.InfiniteTimeSpan)
            {
                timer = null;
                return;
            }

            timer = new Timer(_ => SafeCleanup(), null, runInterval, runInterval);
        }

        private void SafeCleanup()
        {
            if (Interlocked.Exchange(ref cleaning, 1) == 1) return;
            try { CleanupInactiveSessions(timeout); }
            finally { Interlocked.Exchange(ref cleaning, 0); }
        }

        public bool BeginSession(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            if (store.TryGet(userId, out var existing))
            {
                if (existing.Status == SessionStatus.Active)
                {
                    existing.Touch();
                    return true;
                }
            }

            var session = new SessionInfo(userId);
            return store.TryAdd(session);
        }

        public bool TerminateSession(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            if (store.TryGet(userId, out var s))
            {
                s.End();
                store.TryRemove(userId, out _);
                return true;
            }
            return false;
        }

        public bool TouchSession(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            if (store.TryGet(userId, out var s))
            {
                if (s.Status == SessionStatus.Active)
                {
                    s.Touch();
                    return true;
                }
            }
            return false;
        }

        public SessionStatus GetSessionState(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return SessionStatus.None;
            return store.TryGet(userId, out var s) ? s.Status : SessionStatus.None;
        }

        public int CleanupInactiveSessions(TimeSpan timeout)
        {
            int removed = 0;
            foreach (var userId in cleaner.SelectExpired(store, timeout))
            {
                if (store.TryRemove(userId, out var s))
                {
                    s.End();
                    removed++;
                }
            }
            return removed;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
