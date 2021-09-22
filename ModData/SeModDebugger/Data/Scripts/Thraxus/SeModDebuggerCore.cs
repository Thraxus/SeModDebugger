using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sandbox.Game;
using Sandbox.ModAPI;
using SeModDebugger.Thraxus.Common.BaseClasses;
using SeModDebugger.Thraxus.Common.Enums;
using SeModDebugger.Thraxus.Common.Interfaces;
using SeModDebugger.Thraxus.Mods;
using VRage.Game;
using VRage.Game.Components;
using VRageMath;

namespace SeModDebugger.Thraxus
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, priority: int.MinValue + 1)]
	public class SeModDebuggerCore : BaseSessionComp
	{
		protected override string CompName { get; } = nameof(SeModDebuggerCore);
		protected override CompType Type { get; } = CompType.Server;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.BeforeSimulation;

		private readonly List<ICommon> _commonClasses = new List<ICommon>() { };
		private AwwScrap _awwScrap;


		protected override void EarlySetup()
		{
			base.EarlySetup();
			MyAPIGateway.Utilities.MessageEntered += ChatMessageHandler;
		}

		protected override void LateSetup()
		{
			base.LateSetup();
			_awwScrap = new AwwScrap();
			SetupCommonClass(_awwScrap);
		}

		private void SetupCommonClass(ICommon ic)
		{
			ic.OnClose += Close;
			ic.OnWriteToLog += WriteToLog;
			_commonClasses.Add(ic);
		}

		private void TearDownCommonClasses()
		{
			for (var i = _commonClasses.Count - 1; i >= 0; i--)
			{
				var cc = _commonClasses[i];
				cc.Close();
			}
		}

		protected void Close(ICommon ic)
		{
			ic.OnClose -= Close;
			ic.OnWriteToLog -= WriteToLog;
			_commonClasses.Remove(ic);
		}

		private void ChatMessageHandler(string messageText, ref bool sendToOthers)
		{
			//ShowOnScreenDebug(messageText);
			if (!messageText.StartsWith("/debug "))
			{
				sendToOthers = true;
				return;
			}

			string[] split = messageText.Split(' ');
			if (split.Length < 3) return;
			if (string.Equals(split[1], AwwScrap.ChatHandle, StringComparison.InvariantCultureIgnoreCase))
			{
				ShowOnScreenDebug(split[2]);
				_awwScrap.HandleChatMessage(split[2]);
			}
		}

		private static void ShowOnScreenDebug(string message)
		{
			//(message, 5000, Color.Red.ToString());
			MyVisualScriptLogicProvider.AddNotificationLocal(message, MyFontEnum.Red, 5);
		}

		protected override void Unload()
		{
			MyAPIGateway.Utilities.MessageEntered -= ChatMessageHandler;
			TearDownCommonClasses();
			base.Unload();
		}

		public override void LoadData()
		{
			base.LoadData();
		}

		public override void BeforeStart()
		{
			base.BeforeStart();
		}

	}
}
