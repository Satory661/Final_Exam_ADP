using System.Collections.Generic;
using SessionManagement.Models;

namespace SessionManagement.Interfaces
{
    public interface ISessionStore
    {
        bool TryGet(string userId, out SessionInfo session);
        bool TryAdd(SessionInfo session);
        bool TryRemove(string userId, out SessionInfo removed);
        IEnumerable<SessionInfo> Enumerate();
    }
}