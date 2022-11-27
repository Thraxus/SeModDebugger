using Sandbox.Game.Entities;
using SeModDebugger.Thraxus.Common.BaseClasses;
using SeModDebugger.Thraxus.Common.Generics;
using SeModDebugger.Thraxus.Common.Interfaces;
using SeModDebugger.Thraxus.Interfaces;
using SeModDebugger.Thraxus.Investigations;
using System.Collections.Generic;
using SeModDebugger.Thraxus.Common.Extensions;
using VRage.Collections;

namespace SeModDebugger.Thraxus.Mods
{
    internal class HostileTakeoverCore : BaseLoggingClass
    {
        private readonly SeModDebuggerCore _seModDebuggerCore;

        private readonly List<ICommon> _commonClasses = new List<ICommon>();
        private readonly HashSet<IRegisterToEvents> _eventManager;

        private readonly ConcurrentCachingHashSet<Construct> _constructs = new ConcurrentCachingHashSet<Construct>();

        public readonly ActionQueue ActionQueue = new ActionQueue();

        public HostileTakeoverCore(SeModDebuggerCore seModDebuggerCore)
        {
            _seModDebuggerCore = seModDebuggerCore;
            _eventManager = new HashSet<IRegisterToEvents>()
            {
                new OnEntityAddMonitor(this)
            };
        }

        public void AddGridToProcessingQueue(MyCubeGrid grid)
        {
            ActionQueue.Add(2, () => ProcessGrid(grid));
        }
        
        public void ProcessGrid(MyCubeGrid grid)
        {
            WriteGeneral(nameof(ProcessGrid), $"Processing Grid: [{grid.EntityId:D20}]");
            //if (_constructs.Count == 0)
            //{
            //    AddConstruct(grid);
            //    return;
            //}

            foreach (var construct in _constructs)
            {
                //WriteGeneral(nameof(ProcessGrid), $"Loop: [{construct.ContainsGrid(grid).ToSingleChar()}] [{construct.IsConnectedToGrid(grid).ToSingleChar()}]");
                //if (construct.ContainsGrid(grid)) return;

                //WriteGeneral(nameof(ProcessGrid), $"Loop: [{construct.IsConnectedToGrid(grid).ToSingleChar()}]");
                if (construct.IsConnectedToGrid(grid))
                {
                    construct.AddGrid(grid);
                    WriteGeneral(nameof(ProcessGrid), $"Current construct count: {_constructs.Count:D4}");
                    return;
                }
            }

            AddConstruct(grid);
            WriteGeneral(nameof(ProcessGrid), $"Current construct count: {_constructs.Count:D4}");
        }

        private void AddConstruct(MyCubeGrid grid)
        {
            Construct construct = new Construct(this);
            SetupCommonClass(construct);
            construct.AddGrid(grid);
            _constructs.Add(construct);
            _constructs.ApplyAdditions();
        }

        public void DeRegisterAllCachedEvents()
        {
            foreach (var hasEvents in _eventManager)
            {
                hasEvents.DeRegisterEvents();
                var common = hasEvents as ICommon;
                if (common == null) continue;
                Close((ICommon)hasEvents);
            }
        }

        public void RegisterCachedEvents<T>() where T : IRegisterToEvents
        {
            //WriteGeneral(nameof(RegisterCachedEvents), $"Registering Events For: {typeof(T)}");
            foreach (var hasEvents in _eventManager)
            {
                if (!(hasEvents is T)) continue;
                ((T)hasEvents).RegisterEvents();
                var common = hasEvents as ICommon;
                if (common == null) continue;
                SetupCommonClass((ICommon)hasEvents);
            }
        }

        private void SetupCommonClass(ICommon ic)
        {
            if (ic.IsRegistered) return;
            ic.OnClose += Close;
            ic.OnWriteToLog += WriteGeneral;
            ic.IsRegistered = true;
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
            ic.OnWriteToLog -= WriteGeneral;
            ic.IsRegistered = false;
            if (_commonClasses.Contains(ic))
                _commonClasses.Remove(ic);
        }

        public void Unload()
        {
            foreach (var construct in _constructs)
            {
                construct.Close();
            }
            TearDownCommonClasses();
            DeRegisterAllCachedEvents();
        }
    }
}
