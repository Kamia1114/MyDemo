using System;
using System.Collections.Generic;
using Battle.Grid;
using Battle.Manager;
using Battle.Player;
using Core.Enum;
using Core.Mgr;
using Core.Table;
using Core.Utils;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    private PlayerViewModel playerViewModel;
    public event Action OnUpdateUI;
    public event Action<string> OnShowMessage;
    /// <summary>
    /// UI状态变化事件
    /// </summary>
    // public event Action<UIState> OnChangeUIState;

    void Start()
    {
        playerViewModel = PlayerManager.Instance.PlayerViewModel;
        InitEvent();
    }

    private void InitEvent()
    {
        // 初始化事件
        /// 玩家变化
        PlayerManager.Instance.OnCurPlayerChanged += HandlePlayIndexChanged;
        /// 目的地变化
        GridManager.Instance.OnUpdateStationDest += HandleUpdateUI;
        /// 轮次变化
        RoundManager.Instance.OnRoundChanged += HandleRoundChanged;
        /// 玩家行动变化
        playerViewModel.OnPlayerActionChanged += HandleActionChanged;
        /// 玩家数据变化
        playerViewModel.OnUpdateUI += HandleUpdateUI;
        playerViewModel.OnShowMessage += HandleMessage;
    }

    ///——————————————————————— 数据获取接口 ———————————————————
    public string Year => RoundManager.Instance.CurYear().ToString();
    public string Month => RoundManager.Instance.CurMonth().ToString();

    public PlayerModel GetCurrentPlayerModel()
    {
        return PlayerManager.Instance.CurrentPlayer;
    }

    /// 当前站点信息获取
    public int GetCurStationGridID()
    {
        return GetCurrentPlayerModel().CurrentGridId;
    }

    public int GetCurStationCityID()
    {
        int gridId = GetCurStationGridID();
        GridCfgTable curGridCfgTable = ConfigManager.GetConfig<GridCfgTable>(gridId);
        return curGridCfgTable.cfgID;
    }

    public string GetCurStationName()
    {
        CityCfgTable currentCityCfgTable = ConfigManager.GetConfig<CityCfgTable>(GetCurStationCityID());
        return currentCityCfgTable.name;
    }
    
    public List<CompanyCfgTable> GetCurStationCompanies()
    {
        CityCfgTable curCityCfg = ConfigManager.GetConfig<CityCfgTable>(GetCurStationCityID());
        if (curCityCfg != null)
        {
            List<CompanyCfgTable> companyList = new();
            foreach (var companyId in curCityCfg.companysID)
            {
                CompanyCfgTable company = ConfigManager.GetConfig<CompanyCfgTable>(companyId);
                if (company != null)
                {
                    companyList.Add(company);
                }
            }
            return companyList;
        }
        return null;
    }

    /// 终点站信息获取

    public int GetDestStationGridID()
    {
        return GridManager.Instance.StationDestGridID;
    }

    public int GetDestStationCityID()
    {
        int gridId = GetDestStationGridID();
        GridCfgTable stationDestGridCfgTable = ConfigManager.GetConfig<GridCfgTable>(gridId);
        return stationDestGridCfgTable.cfgID;
    }

    public string GetDestStationName()
    {
        CityCfgTable destCityCfgTable = ConfigManager.GetConfig<CityCfgTable>(GetDestStationCityID());
        return destCityCfgTable.name;
    }

    /// 获取当前玩家到目标站点的步数
    public int GetTargetSteps()
    {
        PathResult result = PathFinder.FindShortestPath(GetCurrentPlayerModel().CurrentGridId, GetDestStationGridID());
        return result.StepCount;
    }

    public bool IsBeSellCompany(int companyId)
    {
        PlayerManager.Instance.StockMap.TryGetValue(companyId, out var stock);
        return stock != null;
    }

    ///—————————————————————— UI业务逻辑接口 ———————————————————

    /// StartDecide接口

    public int RollDice()
    {
        int result = playerViewModel.RollDice();
        // OnChangeUIState?.Invoke(UIState.Move);
        return result;
    }

    /// EndDecide接口
    /// 
    public void PlayTurn()
    {
        RoundManager.Instance.PlayerNext();
    }

    public void BuyCompany(CompanyCfgTable companyTable)
    {
        playerViewModel.BuyCompany(companyTable);
        HandleUpdateUI();
    }

    ///——————————————————————— Action调用接口 ———————————————————

    /// <summary>
    /// 处理轮次变化
    /// </summary>
    private void HandlePlayIndexChanged()
    {
        OnUpdateUI?.Invoke(); // 修正：加上?防止OnUpdateUI为null时报错
    }

    private void HandleUpdateUI()
    {
        OnUpdateUI?.Invoke(); // 修正：加上?防止OnUpdateUI为null时报错
    }

    private void HandleMessage(string message)
    {
        OnShowMessage?.Invoke(message);
    }

    private void HandleRoundChanged()
    {
        // OnChangeUIState?.Invoke(UIState.Hide);
    }
    
    private void HandleActionChanged(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Idle:
                // OnChangeUIState?.Invoke(UIState.Hide);
                // UIManager.Instance.CloseAllPanels();
                break;
            case PlayerAction.StartDecide:
                // OnChangeUIState?.Invoke(UIState.StartDecide);
                UIManager.Instance.OpenUI(UIName.StartDecide);
                break;
            case PlayerAction.WaitMove:
                UIManager.Instance.OpenUI(UIName.Move);
                // OnChangeUIState?.Invoke(UIState.Move);
                break;
            case PlayerAction.EndDecide:
                GridCfgTable curGridCfg = ConfigManager.GetConfig<GridCfgTable>(GetCurrentPlayerModel().CurrentGridId);
                switch (curGridCfg.type)
                {
                    case GridTypeEnum.城市:
                        // OnChangeUIState?.Invoke(UIState.CityGrid);
                        UIManager.Instance.OpenUI(UIName.City);
                        break;
                    case GridTypeEnum.金钱:
                        UIManager.Instance.OpenUI(UIName.Money);
                        break;
                    case GridTypeEnum.卡牌:
                        UIManager.Instance.OpenUI(UIName.Card);
                        break;
                    // case GridTypeEnum.彩票:
                    //     OnChangeUIState?.Invoke(UIState.MoneyGrid);
                    //     break;
                    // case GridTypeEnum.传送:
                    //     OnChangeUIState?.Invoke(UIState.StartDecide);
                    //     break;
                    // case GridTypeEnum.商店:
                    //     OnChangeUIState?.Invoke(UIState.StartDecide);   
                    //     break;
                    // case GridTypeEnum.事件:
                    //     OnChangeUIState?.Invoke(UIState.StartDecide);
                    //     break;
                    default:
                        // OnChangeUIState?.Invoke(UIState.Hide);
                        break;
                }
                break;
            default:
                // OnChangeUIState?.Invoke(UIState.Hide);
                break;
        }
    }
}
