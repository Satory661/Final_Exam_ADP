using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SessionManagement.Models;
using SessionManagement.Interfaces;

namespace SessionManagement.Stores
{
    public sealed class InMemorySessionStore : ISessionStore
    {
        private readonly ConcurrentDictionary<string, SessionInfo> sessions =
            new ConcurrentDictionary<string, SessionInfo>(StringComparer.Ordinal);

        public bool TryGet(string userId, out SessionInfo session) =>
            sessions.TryGetValue(userId, out session);

        public bool TryAdd(SessionInfo session) =>
            sessions.TryAdd(session.UserId, session);

        public bool TryRemove(string userId, out SessionInfo removed) =>
            sessions.TryRemove(userId, out removed);

        public IEnumerable<SessionInfo> Enumerate() => sessions.Values;
    }
}