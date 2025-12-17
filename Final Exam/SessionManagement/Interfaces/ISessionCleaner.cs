using System;
using System.Collections.Generic;

namespace SessionManagement.Interfaces
{
    public interface ISessionCleaner
    {
        IEnumerable<string> SelectExpired(ISessionStore store, TimeSpan timeout);
    }
}