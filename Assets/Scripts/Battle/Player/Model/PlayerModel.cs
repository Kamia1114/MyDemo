using System;
using System.Collections.Generic;
using System.Linq;
using Battle.Manager;
using Battle.Player;
using Core.Mgr;
using Core.Table;
using UnityEngine;

public class PlayerModel : PlayerModelABS
{

    // 轮到当前玩家操作
    public event Action<bool> OnTurnChanged;
    public event Action OnTargetChanged;
    // 事件：当玩家状态改变时触发
    public event Action<PlayerAction> OnActionChanged;
    // 事件：当玩家金钱改变时触发
    // public event Action<int> OnMoneyChanged;
    public event Action OnUpdateUI;

    public PlayerStateMachine StateMachine { get; private set; }


    /// <summary>
    /// 初始化玩家
    /// </summary>
    public PlayerModel(PlayerData userData)
    {
        index = userData.index;
        userId = userData.id;
        isAI = userData.isAI;
        characterId = userData.characterId;
        CharacterCfgTable characterCfg = ConfigManager.GetConfig<CharacterCfgTable>(characterId);
        icon = characterCfg.model;
        characterName = characterCfg.name;
        diceList = characterCfg.dice;
        // 初始化玩家资产
        property = new Property
        {
            Money = characterCfg.money, // 初始资金
            Cards = characterCfg.card?.Select(cardId => new Card { id = Guid.NewGuid().GetHashCode(), cardId = cardId, times = 1 }).ToList() ?? new List<Card>()
        };
        currentGridId = 1001;//初始发车上海
        movedPath = new List<int> { currentGridId };
        movePath = new List<int> { };
        carRes = $"Car_0{userData.index}"; // 这里可以根据需要配置车ID
        playerAction = PlayerAction.Idle;
        StateMachine = new PlayerStateMachine(this);
        // 初始状态设为Idle
        StateMachine.ChangeState(PlayerAction.Idle);
    }
    
    public bool IsTurn
    {
        get => isTurn;
        set
        {
            if (isTurn == value) return;
            isTurn = value;
            OnTurnChanged?.Invoke(isTurn);
        }
    }

    public int CurrentGridId
    {
        get => currentGridId;
        set
        {
            currentGridId = value;
            OnUpdateUI?.Invoke();
        }
    }

    // 下一个目标格子ID
    public int NextGridId
    {
        get => nextGridId;
        set
        {
            nextGridId = value;
            OnTargetChanged?.Invoke();
        }
    }

    // 剩余步数
    public int RemainSteps
    {
        get => remainSteps;
        set
        {
            remainSteps = value;
            // OnUpdateUI?.Invoke();
        }
    }

    public PlayerAction PlayerAction
    {
        get => playerAction;
        set
        {
            playerAction = value;
            OnActionChanged?.Invoke(playerAction);
        }
    }


    public List<int> MovePath
    {
        get => movePath;
        set => movePath = value;
    }

    public List<int> MovedPath
    {
        get => movedPath;
        set => movedPath = value;
    }

    /// <summary>
    /// 获取玩家当前位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosition()
    {
        GridCfgTable gridCfg = ConfigManager.GetConfig<GridCfgTable>(currentGridId);
        return new Vector3(gridCfg.coord[0] * GameConfig.GridInterval, GameConfig.PlayerOffsetY, gridCfg.coord[1] * GameConfig.GridInterval);
    }

    /// <summary>
    /// 获取玩家下一个位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetNextPosition()
    {
        GridCfgTable gridCfg = ConfigManager.GetConfig<GridCfgTable>(nextGridId);
        return new Vector3(gridCfg.coord[0] * GameConfig.GridInterval, GameConfig.PlayerOffsetY, gridCfg.coord[1] * GameConfig.GridInterval);
    }

    /// <summary>
    /// 金钱变更
    /// </summary>
    public virtual void ChangeMoney(int amount)
    {
        property.Money += amount;
        OnUpdateUI?.Invoke();
    }

    /// <summary>
    /// 添加公司股份资产
    /// </summary>
    public virtual bool BuyCompany(Stock stock)
    {
        if (property.Money < stock.money) return false;
        ChangeMoney(-1 * stock.money);
        property.OwnedCompanys.Add(stock);
        return true;
    }

    /// <summary>
    /// 出售公司股份
    /// </summary>
    /// <param name="companyId"></param>
    public virtual void SellCompany(int companyId)
    {
        var stock = property.OwnedCompanys.Find(s => s.id == companyId);
        if (stock != null)
        {
            ChangeMoney(stock.money);
            RemoveCompany(companyId);
        }
    }

    /// <summary>
    /// 移除公司股份
    /// </summary>
    /// <param name="companyId"></param>
    public virtual void RemoveCompany(int companyId)
    {
        var stock = property.OwnedCompanys.Find(s => s.id == companyId);
        if (stock != null)
        {
            property.OwnedCompanys.Remove(stock);
        }
    }

    /// <summary>
    /// 检查是否拥有指定地块
    /// </summary>
    public virtual bool HasCompanyById(int companyId)
    {
        return property.OwnedCompanys.Exists(stock => stock.id == companyId);
    }

    public virtual int GetCompanyCount()
    {
        return property.OwnedCompanys.Count;
    }

    /// <summary>
    /// 获取所有公司分红总和
    /// </summary>
    public virtual int GetAllCompanysDivvy()
    {
        return property.OwnedCompanys.Sum(s => s.money * s.ratio / 100);
    }

    public virtual int GetMoney()
    {
        return property.Money;
    }

    /// <summary>
    /// 添加道具
    /// </summary>
    public virtual void AddCard(Card card)
    {
        property.Cards.Add(card);
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    public virtual bool UseCard(int id)
    {
        var card = property.Cards.Find(i => i.id == id);
        if (card != null)
        {
            card.times--;
            if (card.times <= 0)
            {
                property.Cards.Remove(card);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除道具
    /// </summary>
    public virtual void RemoveCard(int id)
    {
        var card = property.Cards.Find(i => i.id == id);
        if (card != null)
        {
            property.Cards.Remove(card);
        }
    }

    /// <summary>
    /// 检查是否拥有指定道具
    /// </summary>
    public virtual bool HasCard(int id)
    {
        var card = property.Cards.Find(i => i.id == id);
        return card != null;
    }

    public virtual void Reset()
    {
        userId = 0;
        characterId = 0;
        characterName = string.Empty;
        carRes = string.Empty;
        icon = string.Empty;
        currentGridId = 0;
        playerAction = PlayerAction.Idle;
        property?.Reset();
    }
}
