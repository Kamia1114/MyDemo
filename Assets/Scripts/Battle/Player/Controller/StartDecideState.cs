using Core.Utils;
using UnityEngine;

namespace Battle.Player
{
    // 决策状态（StartDecide）
    public class StartDecideState : IPlayerState
    {
        public int HandleRollDice(PlayerModel player, int diceCount)
        {
            // 掷骰子后切换到等待移动状态
            int result = Random.Range(1 * diceCount, 6 * diceCount + 1);
            player.RemainSteps = result;
            GameUtils.SetTimeout(() =>
            {
                player.StateMachine.ChangeState(PlayerAction.WaitMove);
            }, 1.5f);
            return result;
        }
    }

}
