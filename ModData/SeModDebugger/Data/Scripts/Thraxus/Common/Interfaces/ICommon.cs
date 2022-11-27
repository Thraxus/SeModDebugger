using System;

namespace SeModDebugger.Thraxus.Common.Interfaces
{
	public interface ICommon
	{
		event Action<ICommon> OnClose;
		event Action<string, string> OnWriteToLog;

		void Update(ulong tick);

		bool IsRegistered { get; set; }
		
        void Close();

		void WriteGeneral(string caller, string message);
	}
}
