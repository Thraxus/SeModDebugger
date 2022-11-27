using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Game.Entities;
using SeModDebugger.Thraxus.Common.Extensions;
using SeModDebugger.Thraxus.Common.Interfaces;
using VRage.Collections;
using VRage.Game;
using VRage.Game.ModAPI;

namespace SeModDebugger.Thraxus.Mods
{
    internal class Construct : ICommon
    {
        private readonly HostileTakeoverCore _hostileTakeoverCore;

        public Construct(HostileTakeoverCore hostileTakeoverCore)
        {
            _hostileTakeoverCore = hostileTakeoverCore;
        }

        private readonly ConcurrentCachingHashSet<MyCubeGrid> _grids = new ConcurrentCachingHashSet<MyCubeGrid>();
        private MyCubeGrid _firstGrid;

        private int GetBlockCountForEntireConstruct()
        {
            int count = 0;
            foreach (var grid in _grids)
            {
                count += grid.BlocksCount;
            }
            return count;
        }

        public void AddGrid(MyCubeGrid grid)
        {
            AddGridCollection(grid);
            if (_firstGrid == null)
                SetupFirstGrid(grid);
        }

        private void SetupFirstGrid(MyCubeGrid grid)
        {
            _firstGrid = grid;
            grid.OnHierarchyUpdated += OnHierarchyUpdated;
            grid.OnGridSplit += OnGridSplit;
            WriteGeneral(nameof(SetupFirstGrid), $"First grid in construct assigned to: [{_firstGrid.EntityId:D20}] [{_firstGrid.BlocksCount:D5}] {_firstGrid.DisplayName}");
        }

        private void OnGridSplit(MyCubeGrid originalGrid, MyCubeGrid newGrid)
        {
            bool resetFirstGrid = false;
            _reusableGridCollection.Clear();
            newGrid.GetConnectedGrids(GridLinkTypeEnum.Mechanical, _reusableGridCollection);
            foreach (var grid in _reusableGridCollection)
            {
                if (_grids.Contains(grid))
                    _grids.Remove(grid);
                if (_firstGrid == grid)
                    resetFirstGrid = true;
            }
            _grids.ApplyRemovals();
            if (resetFirstGrid && _grids.Count > 0)
            {
                CloseFirstGrid();
                SetupFirstGrid(_grids.FirstOrDefault());
            }
            WriteGeneral(nameof(OnGridSplit), $"Sending grid for processing: [{newGrid.EntityId}]");
            _hostileTakeoverCore.AddGridToProcessingQueue(newGrid);
        }

        private readonly List<MyCubeGrid> _reusableGridCollection = new List<MyCubeGrid>();

        private void AddGridCollection(MyCubeGrid grid)
        {
            _reusableGridCollection.Clear();
            grid.GetConnectedGrids(GridLinkTypeEnum.Mechanical, _reusableGridCollection);
            //WriteGeneral(nameof(AddGridCollection), $"Preparing to process a grid with {_reusableGridCollection.Count:D2} associated grids.");
            foreach (var tempGrid in _reusableGridCollection)
            {
                _grids.Add(tempGrid);
            }
            _grids.ApplyAdditions();
            //WriteGeneral(nameof(AddGridCollection), $"Processed a construct with {_grids.Count:D2} associated grids.");
        }

        public bool ContainsGrid(MyCubeGrid grid)
        {
            return _grids.Contains(grid);
        }

        public bool IsConnectedToGrid(MyCubeGrid grid)
        {
            WriteGeneral(nameof(IsConnectedToGrid), $"Checking if [{grid.EntityId:D20}] is connected to construct [{_firstGrid.EntityId:D20}]: {grid.IsConnectedTo(_firstGrid, GridLinkTypeEnum.Mechanical).ToSingleChar()}");
            return grid.IsConnectedTo(_firstGrid, GridLinkTypeEnum.Mechanical);
        }

        private void OnHierarchyUpdated(MyCubeGrid grid)
        {
            WriteGeneral(nameof(OnHierarchyUpdated), $"Hierarchy update triggered: [{grid.EntityId:D20}] [{_grids.Count:D2}] [{GetBlockCountForEntireConstruct():D5}] {grid.DisplayName}");
        }

        public bool ContainsGrid(long gridId)
        {
            foreach (var grid in _grids)
            {
                if (grid.EntityId != gridId) continue;
                return true;
            }
            return false;
        }

        public event Action<ICommon> OnClose;
        public event Action<string, string> OnWriteToLog;
        public void Update(ulong tick)
        {
            
        }
        
        public bool IsRegistered { get; set; }

        private void CloseFirstGrid()
        {
            _firstGrid.OnHierarchyUpdated -= OnHierarchyUpdated;
            _firstGrid.OnGridSplit -= OnGridSplit;
        }

        public void Close()
        {
            CloseFirstGrid();
            _grids.Clear();
        }

        public void WriteGeneral(string caller, string message)
        {
            OnWriteToLog?.Invoke($"Construct [{_firstGrid?.EntityId:D20}]: {caller}", message);
        }
    }
}