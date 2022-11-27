using System;
using System.Collections.Generic;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SeModDebugger.Thraxus.Common.BaseClasses;
using SeModDebugger.Thraxus.Interfaces;
using SeModDebugger.Thraxus.Mods;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace SeModDebugger.Thraxus.Investigations
{
    internal class OnEntityAddMonitor : BaseLoggingClass, IRegisterEventsEarly
    {
        private readonly HostileTakeoverCore _hostileTakeoverCore;

        public OnEntityAddMonitor(HostileTakeoverCore hostileTakeoverCore)
        {
            _hostileTakeoverCore = hostileTakeoverCore;
        }

        public void RegisterEvents()
        {
            MyAPIGateway.Entities.OnEntityAdd += OnEntityAdd;
        }

        private void OnEntityAdd(IMyEntity entity)
        {
            WriteGeneral(nameof(OnEntityAdd), $"Entity Added: [{entity.EntityId:D20}] ({entity.GetObjectBuilder()?.TypeId}) {entity.DisplayName}");
            var grid = entity as MyCubeGrid;
            if (grid?.Physics == null || grid.Immune || grid.IsPreview) return;
            _hostileTakeoverCore.AddGridToProcessingQueue(grid);
        }

        public void DeRegisterEvents()
        {
            MyAPIGateway.Entities.OnEntityAdd -= OnEntityAdd;
        }
    }
}
