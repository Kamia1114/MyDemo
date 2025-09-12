using System.Collections.Generic;
using Core.Mgr;
using Core.Table;
using Core.Utils;

namespace Battle.Player
{
    // 等待移动状态（WaitMove）
    public class WaitMoveState : IPlayerState
    {
        public void Enter(PlayerModel player)
        {
            // 进入等待移动状态时的逻辑
            // 例如，显示可移动范围等
            if (player.MovedPath.Count == 0 || player.MovedPath[^1] != player.CurrentGridId)
                player.MovedPath.Add(player.CurrentGridId);
        }
        public bool HandleMoveDirection(PlayerModel player, Direction direction)
        {
            // 实现方向移动逻辑
            GridCfgTable gridCfg = ConfigManager.GetConfig<GridCfgTable>(player.CurrentGridId);
            if (gridCfg == null) return false;

            foreach (var linkedGridId in gridCfg.links)
            {
                var linkedGrid = ConfigManager.GetConfig<GridCfgTable>(linkedGridId);
                if (linkedGrid == null) continue;

                // 方向判断逻辑...
                if (IsMatchDirection(gridCfg, linkedGrid, direction))
                {
                    player.NextGridId = linkedGridId;
                    player.StateMachine.ChangeState(PlayerAction.Moving);
                    return true;
                }
            }
            return false;
        }

        private bool IsMatchDirection(GridCfgTable from, GridCfgTable to, Direction direction)
        {
            // 方向匹配逻辑
            return direction switch
            {
                Direction.Up => to.coord[1] > from.coord[1],
                Direction.Down => to.coord[1] < from.coord[1],
                Direction.Left => to.coord[0] < from.coord[0],
                Direction.Right => to.coord[0] > from.coord[0],
                _ => false,
            };
        }

        public bool HandleMoveToGrid(PlayerModel player, int gridId)
        {
            // 实现路径移动逻辑
            int stepCount = PathFinder.GetStepCount(player.CurrentGridId, gridId);
            if (stepCount != -1 && stepCount <= player.RemainSteps)
            {
                var path = PathFinder.FindShortestPath(player.CurrentGridId, gridId);
                player.MovePath = path.StationIds;
                player.MovePath.RemoveAt(0); // 移除当前格子
                player.NextGridId = path.StationIds[0];
                player.StateMachine.ChangeState(PlayerAction.Moving);
                return true;
            }
            return false;
        }
    }
}