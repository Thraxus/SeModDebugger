using System;

namespace SeModDebugger.Thraxus.Common.Interfaces
{
    public interface ILog
    {
        event Action<string, string> OnWriteToLog;
        void WriteGeneral(string caller, string message);
    }
}
