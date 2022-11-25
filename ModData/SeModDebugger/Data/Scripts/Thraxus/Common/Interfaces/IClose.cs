using System;

namespace SeModDebugger.Thraxus.Common.Interfaces
{
    internal interface IClose
    {
        event Action<IClose> OnClose;
        void Close();
    }
}
