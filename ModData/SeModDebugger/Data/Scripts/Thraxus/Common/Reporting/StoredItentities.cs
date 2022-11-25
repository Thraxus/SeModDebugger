﻿using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI;
using SeModDebugger.Thraxus.Common.Extensions;
using VRage.Game.ModAPI;

namespace SeModDebugger.Thraxus.Common.Reporting
{
    public static class StoredIdentities
    {
		public static StringBuilder Report(StringBuilder sb)
		{
            sb.AppendLine();
			sb.AppendFormat("{0, -2}Stored Identities", " ");
            sb.AppendLine("__________________________________________________");
            sb.AppendLine();

			var identityList = new List<IMyIdentity>();
			
            MyAPIGateway.Players.GetAllIdentites(identityList);

            // SteamId > 0 denotes player; no reason to see / save their ID though
			sb.AppendFormat("{0, -4}[IdentityId][Dead][SteamID > 0] Display Name\n", " ");
            foreach (IMyIdentity identity in identityList)
                sb.AppendFormat("{0, -4}[{1:D18}][{2}][{3}] {4}\n", " ", identity.IdentityId, identity.IsDead.ToSingleChar(), (MyAPIGateway.Players.TryGetSteamId(identity.IdentityId) > 0).ToSingleChar(), identity.DisplayName);

            return sb;
		}
	}
}
