using Battle.Manager;

namespace Battle.Player
{
    // 移动中状态（Moving）
    public class MovingState : IPlayerState
    {

        public void Enter(PlayerModel player)
        {
            // 进入移动中状态时，直接切换到到达状态
            BattleManager.Instance.HideNearGridTip();
        }
    }

}