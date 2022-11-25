using System;

namespace SeModDebugger.Thraxus.Common.Interfaces
{
    internal interface IReset
    {
        event Action<IReset> OnReset;
        void Reset();
    }
}
