using UnityEngine;
using System;
using System.Collections.Generic;
using Core.Table;
using Core.Mgr;
using Core.Utils;
using System.Collections;
using Battle.Manager;

namespace Battle.Player
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// 负责业务逻辑、状态变更、事件分发（如玩家移动、回合切换、属性变更通知）。
    /// 监听PlayerManager的数据变化，处理业务逻辑，并通过事件通知View层。
    /// 只持有PlayerModel的引用，不直接操作GameObject。
    /// </summary>
    public class PlayerViewModel
    {
        private readonly PlayerManager playerManager;
        public event Action OnUpdateUI;
        public event Action<string> OnShowMessage;
        public event Action<PlayerAction> OnPlayerActionChanged;
        public PlayerViewModel(PlayerManager manager)
        {
            playerManager = manager;
        }

        public void BindPlayer(PlayerModel playerModel, PlayerItem playerItem)
        {
            // playerManager.AddPlayerModel(playerModel);
            // 初始设置
            playerItem.OnCurPlayerTurnChanged(playerModel.IsTurn);
            // 注册更新事件
            playerModel.OnTurnChanged += (isTurn) => playerItem.OnCurPlayerTurnChanged(isTurn);
            playerModel.OnTargetChanged += () => OnPlayerMoved(playerModel, playerItem);
            playerModel.OnActionChanged += (action) => OnPlayerActionChanged?.Invoke(action);
            playerModel.OnUpdateUI += () => OnUpdateUI?.Invoke();
        }

        /// <summary>
        /// 目标格子变化时调用
        /// </summary>
        private void OnPlayerMoved(PlayerModel playerModel, PlayerItem playerItem)
        {
            playerItem.OnPlayerPositionChanged(playerModel.GetNextPosition());
        }

        public Vector3 GetPlayerPosition(int index)
        {
            var model = playerManager.GetPlayerModel(index);
            return model != null ? model.GetPosition() : Vector3.zero;
        }

        //---------------------------------------------------------------
        /// 业务逻辑调用接口

        public int RollDice(int diceCount)
        {
            int result = playerManager.CurrentPlayer.StateMachine.HandleRollDice(diceCount);
            return result;
        }

        /// <summary>
        /// 按照方向移动
        /// </summary>
        /// <param name="direction"></param>
        public void MoveDirection(Direction direction)
        {
            playerManager.CurrentPlayer.StateMachine.HandleMoveDirection(direction);
        }

        /// <summary>
        /// 移动到指定的格子
        /// </summary>
        /// <param name="gridId"></param>
        public void MoveToGrid(int gridId)
        {
            playerManager.CurrentPlayer.StateMachine.HandleMoveToGrid(gridId);
        }
        
        public void UseCard(int cardOnlyId)
        {
            string message = "";
            var isUse = playerManager.PlayerUseCard(cardOnlyId);
            if (isUse)
            {
                OnUpdateUI?.Invoke();
                message = "卡牌使用成功";
            }
            else
            {
                message = "卡牌使用失败";
            }
            OnShowMessage?.Invoke(message);
        }

        public void BuyCompany(CompanyCfgTable companyTable)
        {
            string message = "";
            if (PlayerManager.Instance.StockMap.ContainsKey(companyTable.ID))
            {
                Debug.LogWarning("该公司股票已被购买");
                message = "该公司股票已被购买";
            }
            else if (PlayerManager.Instance.CurrentPlayer.PlayerAssets.Money < companyTable.money)
            {
                Debug.LogWarning("钱不够，无法购买");
                if (companyTable.money - playerManager.CurrentPlayer.PlayerAssets.Money < 5000)
                {
                    message = $"{playerManager.CurrentPlayer.CharacterName}!\n钱不够了\n再攒攒吧";
                }
                else
                {
                    message = $"{playerManager.CurrentPlayer.CharacterName}!\n别光喝酒啊\n吃点花生米吧";
                }
            }
            else
            {
                var isBuy = playerManager.PlayerBuyCompany(companyTable);
                if (isBuy)
                {
                    OnUpdateUI?.Invoke();
                    message = "买下啦\n看看下一家";
                }
            }
            OnShowMessage?.Invoke(message);
        }

        public void NotifyPlayerArrived()
        {
            Debug.Log("PlayerViewModel: 玩家到达目的地");
            /// 到达目标格子
            playerManager.CurrentPlayer.StateMachine.ChangeState(PlayerAction.Arrived);
        }
    }
}