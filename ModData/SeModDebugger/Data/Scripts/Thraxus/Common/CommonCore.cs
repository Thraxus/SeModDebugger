﻿using SeModDebugger.Thraxus.Common.BaseClasses;
using SeModDebugger.Thraxus.Common.Enums;
using SeModDebugger.Thraxus.Common.Factions.Models;
using SeModDebugger.Thraxus.Common.Reporting;
using VRage.Game.Components;

namespace SeModDebugger.Thraxus.Common
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, priority: int.MinValue + 1)]
	public class CommonCore : BaseSessionComp
	{
		protected override string CompName { get; } = "CommonCore";
		protected override CompType Type { get; } = CompType.Server;
		protected override MyUpdateOrder Schedule { get; } = MyUpdateOrder.NoUpdate;

		protected override void SuperEarlySetup()
		{
			base.SuperEarlySetup();
		}

		protected override void LateSetup()
		{
			base.LateSetup();
			FactionDictionaries.Initialize();
			WriteToLog($"{CompName} - Game Settings", $"{GameSettings.Report()}", LogType.General);
			WriteToLog($"{CompName} - Factions", $"{FactionDictionaries.Report()}", LogType.General);
		}
	}
}
