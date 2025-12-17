using System;
using System.Collections.Generic;
using SessionManagement.Interfaces;
using SessionManagement.Models;

namespace SessionManagement.Cleaners
{
    public sealed class InactivitySessionCleaner : ISessionCleaner
    {
        public IEnumerable<string> SelectExpired(ISessionStore store, TimeSpan timeout)
        {
            var now = DateTime.UtcNow;
            foreach (var session in store.Enumerate())
            {
                if (session.Status != SessionStatus.Active) continue;
                var idle = now - session.LastActive;
                if (idle >= timeout)
                    yield return session.UserId;
            }
        }
    }
}