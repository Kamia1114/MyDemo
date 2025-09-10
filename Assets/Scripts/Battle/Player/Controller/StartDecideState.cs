using UnityEngine;

namespace Battle.Player
{
    // 决策状态（StartDecide）
    public class StartDecideState : IPlayerState
    {
        public int HandleRollDice(PlayerModel player)
        {
            // 掷骰子后切换到等待移动状态
            int result = Random.Range(1, 7);
            player.RemainSteps = result;
            player.StateMachine.ChangeState(PlayerAction.WaitMove);
            return result;
        }
    }

}
