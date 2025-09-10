using Battle.Manager;
using UnityEngine;

namespace Battle.Player
{
    // 决策状态（EndDecide）
    public class EndDecideState : IPlayerState
    {
        public void Enter(PlayerModel player)
        {
            if (GridManager.Instance.StationDestGridID == player.CurrentGridId)
            {
                player.StateMachine.ChangeState(PlayerAction.Idle);
                /// 已经到达
                BattleManager.Instance.ArrivedStationDest();
            }
        }
    }

}
