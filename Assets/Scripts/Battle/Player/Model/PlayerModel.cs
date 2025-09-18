using System;
using System.Collections.Generic;
using System.Linq;
using Battle.Manager;
using Battle.Player;
using Battle.Skill;
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
        playerAssets = new PlayerAssets
        {
            Money = characterCfg.money, // 初始资金
            Cards = characterCfg.card?.Select(cardId => CardManager.Instance.CreateCardModel(cardId, index)).ToList() ?? new List<CardModel>()
        };
        currentGridId = int.Parse(ConfigManager.GetGameConfig("StartPoint"));//初始发车上海
        movedPath = new List<int> { };
        movePath = new List<int> { };
        skills = new List<SkillBase>();
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
    public void ChangeMoney(int amount)
    {
        playerAssets.Money += amount;
        OnUpdateUI?.Invoke();
    }

    /// <summary>
    /// 添加公司股份资产
    /// </summary>
    public bool BuyCompany(Stock stock)
    {
        if (playerAssets.Money < stock.money) return false;
        ChangeMoney(-1 * stock.money);
        playerAssets.OwnedCompanys.Add(stock);
        return true;
    }

    /// <summary>
    /// 出售公司股份
    /// </summary>
    /// <param name="companyId"></param>
    public void SellCompany(int companyId)
    {
        var stock = playerAssets.OwnedCompanys.Find(s => s.id == companyId);
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
    public void RemoveCompany(int companyId)
    {
        var stock = playerAssets.OwnedCompanys.Find(s => s.id == companyId);
        if (stock != null)
        {
            playerAssets.OwnedCompanys.Remove(stock);
        }
    }

    /// <summary>
    /// 检查是否拥有指定地块
    /// </summary>
    public bool HasCompanyById(int companyId)
    {
        return playerAssets.OwnedCompanys.Exists(stock => stock.id == companyId);
    }

    public int GetCompanyCount()
    {
        return playerAssets.OwnedCompanys.Count;
    }

    /// <summary>
    /// 获取所有公司分红总和
    /// </summary>
    public int GetAllCompanysDivvy()
    {
        return playerAssets.OwnedCompanys.Sum(s => s.money * s.ratio / 100);
    }

    public int GetMoney()
    {
        return playerAssets.Money;
    }

    /// <summary>
    /// 添加卡牌
    /// </summary>
    public void AddCard(int cardId)
    {
        playerAssets.Cards.Add(CardManager.Instance.CreateCardModel(cardId, index));
    }

    /// <summary>
    /// 通过位移ID获取卡牌
    /// </summary>
    public CardModel GetCard(int id)
    {
        return playerAssets.Cards.Find(i => i.ID == id);
    }

    /// <summary>
    /// 使用卡牌
    /// </summary>
    public bool UseCard(int id)
    {
        var card = playerAssets.Cards.Find(i => i.ID == id);
        if (card != null)
        {
            card.remainingTimes--;
            if (card.remainingTimes <= 0)
            {
                playerAssets.Cards.Remove(card);
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
        var card = playerAssets.Cards.Find(i => i.ID == id);
        if (card != null)
        {
            playerAssets.Cards.Remove(card);
        }
    }

    /// <summary>
    /// 检查是否拥有指定道具
    /// </summary>
    public virtual bool HasCard(int id)
    {
        var card = playerAssets.Cards.Find(i => i.ID == id);
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
        playerAssets?.Reset();
    }
}
