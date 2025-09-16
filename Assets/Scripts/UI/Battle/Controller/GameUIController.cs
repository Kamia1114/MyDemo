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

    public string GetCurrentPlayerName()
    {
        return GetCurrentPlayerModel().CharacterName;
    }

    // 
    public PlayerAssets GetCurrentPlayerProperty()
    {
        return GetCurrentPlayerModel().PlayerAssets;
    }

    /// 是否已经被买过了
    public bool IsBeBuyCompany(int companyId)
    {
        return PlayerManager.Instance.StockMap.ContainsKey(companyId);
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

    /// <summary>
    /// 获取最短路径的引导格子
    /// </summary>
    /// <returns></returns>
    public List<int> GetNearestGrids()
    {
        var paths = PathFinder.FindAllShortestPaths(GetCurrentPlayerModel().CurrentGridId, GetDestStationGridID());
        var nearestGrids = new List<int>();
        foreach (var path in paths)
        {
            if (path.Found)
            {
                Debug.Log($"path.StepCount: {path.StepCount}");
                nearestGrids.Add(path.StationIds[1]);
            }
        }
        return nearestGrids;
    }

    ///—————————————————————— UI业务逻辑接口 ———————————————————

    /// StartDecide接口

    public int RollDice()
    {
        int result = playerViewModel.RollDice();
        return result;
    }

    public void UseCard(int cardOnlyId)
    {
        playerViewModel.UseCard(cardOnlyId);
    }

    /// EndDecide接口
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
    
    private void HandleActionChanged(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Idle:
                break;
            case PlayerAction.StartDecide:
                UIManager.Instance.OpenUI(UIName.StartDecide);
                break;
            case PlayerAction.WaitMove:
                UIManager.Instance.OpenUI(UIName.Move);
                break;
            case PlayerAction.EndDecide:
                GridCfgTable curGridCfg = ConfigManager.GetConfig<GridCfgTable>(GetCurrentPlayerModel().CurrentGridId);
                switch (curGridCfg.type)
                {
                    case GridTypeEnum.City:
                        UIManager.Instance.OpenUI(UIName.City);
                        break;
                    case GridTypeEnum.Money:
                        var curType = curGridCfg.arg == "+"?MoneyType.Income:MoneyType.Expense;
                        UIManager.Instance.OpenUI(UIName.Money, new object[] { curType });
                        break;
                    case GridTypeEnum.Card:
                        UIManager.Instance.OpenUI(UIName.Card);
                        break;
                    default:
                        Debug.LogWarning($"未知格子类型: {curGridCfg.type}");
                        break;
                }
                break;
            default:
                Debug.LogWarning($"未知角色状态类型: {action}");
                break;
        }
    }
}
