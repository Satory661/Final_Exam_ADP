using System;

namespace SessionManagement.Models
{
    public sealed class SessionInfo
    {
        public string UserId { get; }
        public SessionStatus Status { get; private set; }
        public DateTime Created { get; }
        public DateTime LastActive { get; private set; }

        public SessionInfo(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Status = SessionStatus.Active;
            Created = DateTime.UtcNow;
            LastActive = Created;
        }

        public void Touch()
        {
            if (Status == SessionStatus.Active)
                LastActive = DateTime.UtcNow;
        }

        public void End()
        {
            Status = SessionStatus.Terminated;
        }
    }
}