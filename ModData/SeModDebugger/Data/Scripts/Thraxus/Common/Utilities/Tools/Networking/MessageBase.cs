﻿using ProtoBuf;
using Sandbox.ModAPI;
using SeModDebugger.Thraxus.Common.Utilities.Tools.Networking.Messages;

namespace SeModDebugger.Thraxus.Common.Utilities.Tools.Networking
{
	[ProtoInclude(10, typeof(ExampleMessage))]
	[ProtoContract]
	public abstract class MessageBase
	{
		[ProtoMember(1)] private readonly ulong _senderId;

		protected MessageBase()
		{
			_senderId = MyAPIGateway.Multiplayer.MyId;
		}

		public abstract void HandleServer();

		public abstract void HandleClient();
	}
}
