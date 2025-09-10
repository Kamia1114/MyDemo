using Core.Utils;
using UnityEngine;
using System;
using Core.Mgr;

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
            InitEvent();
        }

        private void InitEvent()
        {
            roundManager.OnRoundChanged += HandleRoundChanged;
            roundManager.OnRoundOver += HandleRoundOver;
            roundManager.OnPlayIndexChanged += HandlePlayIndexChanged;

            EventManager.AddListener(GameEvent.OnGridEvent, OnHandlerGridEvent);
            EventManager.AddListener(GameEvent.OnPlayerEvent, OnHandlerPlayerEvent);
        }

        public void GameStart()
        {
            Debug.Log("游戏开始！");
            // OnGameStart?.Invoke();
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

        private void HandleRoundChanged()
        {
            Debug.Log("模拟播放一个回合特效");
            GameUtils.SetTimeout(1.0f, () =>
            {
                Debug.Log("回合特效播放完毕");
                roundManager.PlayerStart();
            });
        }

        private void HandleRoundOver()
        {
            Debug.Log("回合结束，进行结算");
            int SettlementPeriod = int.Parse(ConfigManager.GetGameConfig("Settlement"));
            if (SettlementPeriod % roundManager.CurRound == 0)
            {
                //结算周期到了
                playerManager.Settlement();
            }
            else
            {
                //结算周期没到，直接进入下一回合
                roundManager.NextRound();
            }
        }

        public void SettlementEnd()
        {
            //结算结束，进入下一回合
            roundManager.NextRound();
        }

        private void HandlePlayIndexChanged(int index)
        {
            Debug.Log($"当前玩家索引: {index}");
            playerManager.PlayTurn(index);
        }

        private void OnHandlerGridEvent(object data)
        {
            if (data is GridEventObject gridEvent)
            {
                playerManager.OnHandlerGridEvent(gridEvent);
            }
        }

        private void OnHandlerPlayerEvent(object data)
        {
            if (data is PlayerEventObject playerEvent)
            {
                
            }
        }
    }
}

