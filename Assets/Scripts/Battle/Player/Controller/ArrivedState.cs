using System.Collections.Generic;
using Battle.Manager;
using UnityEngine;

namespace Battle.Player
{
    // 到达状态（Arrived）
    public class ArrivedState : IPlayerState
    {
        public void Enter(PlayerModel player)
        {
            if (player.MovedPath != null && player.MovedPath.Count >= 2 && player.MovedPath[^2] == player.NextGridId)
            {
                /// 回退一步
                player.RemainSteps++;
                player.MovedPath.RemoveAt(player.MovedPath.Count - 1);
            }
            else
            {
                /// 消耗一步
                player.RemainSteps--;
                if (player.RemainSteps == 0)
                {
                    player.MovedPath.Clear();
                    player.MovedPath.Add(player.NextGridId);
                }
                else
                {
                    player.MovedPath.Add(player.NextGridId);
                }
            }
            player.CurrentGridId = player.NextGridId;
            if (player.MovePath.Count > 0)
            {
                player.MovePath.RemoveAt(0);
            }
            // 剩余步数没走完
            if (player.RemainSteps > 0)
            {
                if (player.MovePath.Count > 0)
                {
                    // 移动路径还有未走完的，继续移动
                    player.NextGridId = player.MovePath[0];
                    player.StateMachine.ChangeState(PlayerAction.Moving);
                }
                else
                {
                    // 目标路径走完了，步数没用完，等待操作移动
                    player.StateMachine.ChangeState(PlayerAction.WaitMove);
                    BattleManager.Instance.UpdateNearGridTip();
                }
            }
            else
            {
                Debug.Log("玩家步数走完，进入决策处理");
                player.StateMachine.ChangeState(PlayerAction.EndDecide);
            }
        }
    }
}