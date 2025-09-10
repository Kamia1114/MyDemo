namespace Battle.Player
{
    // 玩家状态接口
    public interface IPlayerState
    {
        void Enter(PlayerModel player) { }
        void Update(PlayerModel player) { }
        void Exit(PlayerModel player) { }

        // 处理移动指令
        bool HandleMoveDirection(PlayerModel player, Direction direction) { return false; }
        bool HandleMoveToGrid(PlayerModel player, int gridId) { return false; }

        // 处理掷骰子
        int HandleRollDice(PlayerModel player) { return -1; }
    }
}