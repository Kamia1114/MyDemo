using Core.Utils;
using UnityEngine;
using System;
using Core.Mgr;
using Core.Enum;

namespace Battle.Manager
{
    /// <summary>
    /// 只负责游戏主流程调度（如回合推进、游戏开始/结束、事件分发）。
    /// 不直接管理具体数据和表现，只协调各Manager
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private RoundManager roundManager;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private GridManager gridManager;

        public static BattleManager Instance { get; private set; }

        // public event Action OnGameStart;
        // public event Action OnGameOver;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void Start()
        {
            Init();
            Invoke(nameof(GameStart), 1.0f);
        }

        private void Init()
        {
            SkillManager.Instance.Register();
            InitEvent();
        }

        private void InitEvent()
        {
            roundManager.OnRoundOver += HandleRoundOver;
            roundManager.OnPlayIndexChanged += HandlePlayIndexChanged;
            EventManager.AddListener(GameEvent.OnBattleEvent, OnHandlerBattleEvent);
        }

        public void GameStart()
        {
            Debug.Log("游戏开始！");
            gridManager.RandomNextStationDest();
            roundManager.StartGame();
        }

        /// 玩家到达目的地
        public void ArrivedStationDest()
        {
            UIManager.Instance.OpenUI(UIName.Dest);
        }

        public void ArrivedResult()
        {
            playerManager.HandlePlayerArrived();
            gridManager.RandomNextStationDest();
        }

        public void GameOver()
        {
            Debug.Log("游戏结束！");
            // OnGameOver?.Invoke();
        }

        private void HandleRoundOver()
        {
            Debug.Log("回合结束，进行结算");
            EventCenter.Instance.Trigger(TriggerTimingEnum.TurnEnd);
            int SettlementPeriod = int.Parse(ConfigManager.GetGameConfig("Settlement"));
            if (roundManager.CurRound % SettlementPeriod == 0)
            {
                //结算周期到了
                playerManager.Settlement();
            }
            else
            {
                //结算周期没到，直接进入下一回合
                GameUtils.SetTimeout(() =>
                {
                    //确保结算界面关闭后再开始下一回合
                    EventCenter.Instance.Trigger(TriggerTimingEnum.TurnStart);
                    roundManager.NextRound();
                }, 0.5f);
            }
        }

        public void SettlementEnd()
        {
            //结算结束，进入下一回合
            EventCenter.Instance.Trigger(TriggerTimingEnum.TurnStart);
            roundManager.NextRound();
        }

        private void HandlePlayIndexChanged(int index)
        {
            Debug.Log($"当前玩家索引: {index}");
            playerManager.PlayTurn(index);
            gridManager.ShowNearGridTip(playerManager.CurrentPlayer);
        }

        public void HideNearGridTip()
        {
            gridManager.HideNearGridTip();
        }

        public void UpdateNearGridTip()
        {
            //更新附近格子提示
            gridManager.ShowNearGridTip(playerManager.CurrentPlayer);
        }

        private void OnHandlerBattleEvent(EventStruct data)
        {
            Debug.Log("收到游戏中事件");
            switch (data.eventType)
            {
                case EventEnum.OnGridClicked:
                    playerManager.OnHandlerEvent(data);
                    break;
                case EventEnum.OnShowMessage:
                    UIManager.Instance.ShowMessage(data.args as string);
                    break;
                default:
                    Debug.LogWarning("未处理的游戏事件: " + data.eventType);
                    break;
            }
        }
        
    }
}

