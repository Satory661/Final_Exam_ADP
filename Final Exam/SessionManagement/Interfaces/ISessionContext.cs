using System;
using SessionManagement.Models;

namespace SessionManagement.Interfaces
{
    public interface ISessionContext
    {
        bool BeginSession(string userId);
        bool TerminateSession(string userId);
        bool TouchSession(string userId);
        SessionStatus GetSessionState(string userId);
        int CleanupInactiveSessions(TimeSpan timeout);
    }
}