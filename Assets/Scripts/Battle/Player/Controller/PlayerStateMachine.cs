using System.Collections.Generic;

namespace Battle.Player
{
    /// <summary>
    /// 玩家状态机类，管理玩家的不同状态及其转换
    /// </summary>
    public class PlayerStateMachine
    {
        private readonly PlayerModel player;
        private readonly Dictionary<PlayerAction, IPlayerState> states = new();
        private IPlayerState currentState;

        public PlayerStateMachine(PlayerModel player)
        {
            this.player = player;
            // 注册所有状态
            states[PlayerAction.Idle] = new IdleState();
            states[PlayerAction.StartDecide] = new StartDecideState();
            states[PlayerAction.WaitMove] = new WaitMoveState();
            states[PlayerAction.Moving] = new MovingState();
            states[PlayerAction.Arrived] = new ArrivedState();
            states[PlayerAction.EndDecide] = new EndDecideState();
        }

        public void ChangeState(PlayerAction action)
        {
            if (player.PlayerAction == action)
                return;
            currentState?.Exit(player);
            player.PlayerAction = action;
            currentState = states[action];
            currentState.Enter(player);
        }

        // 转发状态处理方法
        public bool HandleMoveDirection(Direction direction) => currentState.HandleMoveDirection(player, direction);
        public bool HandleMoveToGrid(int gridId) => currentState.HandleMoveToGrid(player, gridId);
        public int HandleRollDice(int diceCount) => currentState.HandleRollDice(player, diceCount);
        public void Update() => currentState.Update(player);
    }
}