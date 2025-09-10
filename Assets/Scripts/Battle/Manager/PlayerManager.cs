using System;
using System.Collections;
using System.Collections.Generic;
using Battle.Player;
using Core.Enum;
using Core.Mgr;
using Core.Table;
using Core.Utils;
using UnityEngine;

namespace Battle.Manager
{
    [Serializable]
    public struct PlayerData
    {
        public int index;
        public int id;
        public int characterId;
        public bool isAI;
    }

    public struct PlayerEventObject
    {
        public PlayerEventEnum eventType;
        public int playerId;
    }

    /// <summary>
    /// 只负责玩家数据的创建、存储、查找（如PlayerModel的增删查）。
    /// 不处理UI、不直接操作GameObject。
    /// 提供玩家数据的增删查改接口，管理所有PlayerModel。
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject carContainer;
        private PlayerViewModel playerViewModel;
        public PlayerViewModel PlayerViewModel => playerViewModel;
        public static PlayerManager Instance { get; private set; }
        private readonly Dictionary<int, PlayerModel> modelMap = new();
        public Dictionary<int, PlayerModel> ModelMap { get { return modelMap; } }
        private PlayerModel currentPlayer;
        public event Action OnCurPlayerChanged;
        /// <summary>
        /// 玩家所持有的公司股票信息
        /// </summary>
        public readonly Dictionary<int, Stock> StockMap = new();
        public readonly Dictionary<int, Stock> PlayerStockMap = new();
        public readonly Dictionary<int, int> CityStockMap = new();

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
            InitEvent();
        }

        public void Init()
        {
            playerViewModel = new PlayerViewModel(this);
            var cameraScript = Camera.main.GetComponent<CameraFollowPlayer>();
            cameraScript.SetPlayerViewModel(playerViewModel);
            CreatePlayers();
        }

        public void InitEvent()
        {
            playerViewModel.onPlayerEvent += OnHandlerPlayerEvent;
            StartCoroutine(ArrowKeyListenerCoroutine());
        }

        public void OnHandlerPlayerEvent(PlayerEventObject eventData)
        {
            EventManager.TriggerEvent(GameEvent.OnPlayerEvent, eventData);
        }

        public void OnHandlerGridEvent(GridEventObject eventData)
        {
            switch (eventData.eventType)
            {
                case GridEventEnum.OnGridClicked:
                    /// 只有当前玩家才响应格子点击事件
                    if (currentPlayer != null && currentPlayer.CurrentGridId != eventData.gridId && currentPlayer.NextGridId != eventData.gridId)
                    {
                        currentPlayer.StateMachine.HandleMoveToGrid(eventData.gridId);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 持续监听键盘方向键
        /// </summary>
        private IEnumerator ArrowKeyListenerCoroutine()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    playerViewModel.MoveDirection(Direction.Up);
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    playerViewModel.MoveDirection(Direction.Down);
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    playerViewModel.MoveDirection(Direction.Left);
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    playerViewModel.MoveDirection(Direction.Right);
                yield return null;
            }
        }

        ///-------------------------------- 玩家创建与玩家数据获取 --------------------------------

        private void CreatePlayers()
        {
            foreach (var playerData in GameManager.Instance.PlayerDataList)
            {
                var carItemPrefab = CreateManager.CreateCarPrefab($"Car_0{playerData.index + 1}");
                if (!carItemPrefab)
                {
                    Debug.LogError($"未能创建车模型: Car_0{playerData.index + 1}");
                    continue;
                }
                var carItem = Instantiate(carItemPrefab, carContainer.transform, false);
                carItem.name = $"User_{playerData.id}";
                carItem.SetActive(false); // 初始隐藏，防止叠堆
                var playerModel = new PlayerModel(playerData);
                ModelMap.Add(playerData.index, playerModel);
                var playerItem = carItem.AddComponent<PlayerItem>();
                playerItem.SetData(playerData.index, playerViewModel);
                playerViewModel.BindPlayer(playerModel, playerItem);
            }
        }

        public PlayerModel GetPlayerModel(int index)
        {
            if (ModelMap.TryGetValue(index, out var model))
                return model;
            return null;
        }

        public PlayerModel CurrentPlayer => currentPlayer;

        public IEnumerable<PlayerModel> GetAllPlayerModels()
        {
            return ModelMap.Values;
        }

        ///———————————————— BattleManager接口调用 ————————————————

        public void PlayTurn(int index)
        {
            Debug.Log("轮次开始！");
            foreach (var model in GetAllPlayerModels())
            {
                if (model.Index == index)
                {
                    model.IsTurn = true;
                    currentPlayer = model;
                    model.StateMachine.ChangeState(PlayerAction.StartDecide);
                }
                else
                {
                    model.IsTurn = false;
                    model.StateMachine.ChangeState(PlayerAction.Idle);
                }
            }
            // playerViewModel.PlayTurn();
            OnCurPlayerChanged?.Invoke();
        }


        public void HandlePlayerArrived()
        {
            Debug.Log("玩家到达目的地，通知回合管理器");
            if (currentPlayer.PlayerProperty.Money < 0)
            {
                currentPlayer.PlayerProperty.Money = 0;
            }
            else
            {
                currentPlayer.PlayerProperty.Money += 2000; // 到达目的地奖励200
            }
        }

        /// <summary>
        /// 玩家购买公司股票
        /// </summary>
        /// <param name="companyTable"></param>
        public bool PlayerBuyCompany(CompanyCfgTable companyTable)
        {
            var curCityId = ConfigManager.GetConfig<CityCfgTable>(currentPlayer.CurrentGridId).ID;
            Stock stock = new()
            {
                id = companyTable.ID,
                cityId = curCityId,
                money = companyTable.money,
                ratio = companyTable.ratio,
                isOwned = true,
                ownerId = currentPlayer.Index
            };
            if (currentPlayer.BuyCompany(stock))
            {
                StockMap[stock.id] = stock;
                PlayerStockMap[stock.ownerId] = stock;
                CityStockMap[stock.cityId] = stock.id;
                Debug.Log($"玩家{currentPlayer.CharacterName}购买公司股票成功: 公司ID {stock.id}, 价格 {stock.money}");
                return true;
            }
            else
            {
                Debug.LogWarning("玩家购买公司失败，资金不足");
                return false;
            }
        }

        /// <summary>
        /// 结算资产收益
        /// </summary>
        public void Settlement()
        {
            Debug.Log("进行结算");
            foreach (var model in GetAllPlayerModels())
            {
                int totalIncome = 0;
                foreach (var stock in model.PlayerProperty.OwnedCompanys)
                {
                    int income = (int)(stock.money * stock.ratio);
                    totalIncome += income;
                    Debug.Log($"玩家 {model.CharacterName} 持有公司ID {stock.id} 获得分红 {income}");
                }
                if (totalIncome > 0)
                {
                    model.ChangeMoney(totalIncome);
                    Debug.Log($"玩家 {model.CharacterName} 本次结算总收入 {totalIncome}");
                }
                else
                {
                    Debug.Log($"玩家 {model.CharacterName} 本次结算无收入");
                }
            }
            GameUtils.SetTimeout(2.0f, () =>
            {
                Debug.Log("结算完毕，通知BattleManager");
                BattleManager.Instance.SettlementEnd();
            });
        }
    }
}
