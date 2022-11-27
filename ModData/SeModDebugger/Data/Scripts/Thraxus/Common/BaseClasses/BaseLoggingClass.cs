using System;
using SeModDebugger.Thraxus.Common.Interfaces;

namespace SeModDebugger.Thraxus.Common.BaseClasses
{
	public abstract class BaseLoggingClass : ICommon
	{
		public event Action<string, string> OnWriteToLog;
		public event Action<ICommon> OnClose;

		public bool IsClosed { get; private set; }
        public bool IsRegistered { get; set; }

        public virtual void Close()
		{
			if (IsClosed) return;
			IsClosed = true;
			OnClose?.Invoke(this);
		}

		public virtual void Update(ulong tick) { }

		public virtual void WriteGeneral(string caller, string message)
		{
			OnWriteToLog?.Invoke(caller, message);
		}
	}
}