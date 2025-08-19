using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家状态枚举
/// </summary>
public enum PlayerState
{
    Normal,       // 正常状态
    InJail,       // 监禁中
    Bankrupt,     // 破产
    Waiting,      // 等待中
    RollingDice   // 掷骰子中
}

/// <summary>
/// 道具类
/// </summary>
[Serializable]
public class Item
{
    public string itemId;       // 道具ID
    public string itemName;     // 道具名称
    public int quantity;        // 数量
    public string description;  // 描述
    public bool isUsable;       // 是否可使用
}

/// <summary>
/// 玩家抽象基类
/// </summary>
public abstract class BasePlayer : MonoBehaviour
{
    [Header("玩家基本信息")]
    [SerializeField] protected string userId;          // 用户唯一ID
    [SerializeField] protected string characterId;     // 角色ID
    [SerializeField] protected string characterName;   // 角色名称
    [SerializeField] protected int iconId;             // 角色图标ID

    [Header("游戏状态信息")]
    [SerializeField] protected string currentPlotId;   // 当前所在地块ID
    [SerializeField] protected int money;              // 当前拥有的金钱
    [SerializeField] protected PlayerState state;      // 当前状态
    [SerializeField] protected int jailTurns;          // 剩余监禁回合数

    [Header("资产信息")]
    [SerializeField] protected List<int> ownedPlotIds = new List<int>();  // 拥有的地块ID列表
    [SerializeField] protected List<Item> items = new List<Item>();              // 拥有的道具列表

    // 事件：当玩家状态改变时触发
    public event Action<PlayerState> OnStateChanged;
    // 事件：当玩家金钱改变时触发
    public event Action<int> OnMoneyChanged;
    // 事件：当玩家移动时触发
    public event Action<string> OnMoved;

    #region 属性访问器
    public string UserId => userId;
    public string CharacterId => characterId;
    public string CharacterName => characterName;
    public int IconId => iconId;
    public string CurrentPlotId => currentPlotId;
    public int Money => money;
    public PlayerState State => state;
    public int JailTurns => jailTurns;
    public List<int> OwnedPlotIds => new List<int>(ownedPlotIds);
    public List<Item> Items => new List<Item>(items);
    #endregion

    /// <summary>
    /// 初始化玩家
    /// </summary>
    public virtual void Initialize(string userId, string characterId, string characterName, 
                          int iconId, int startMoney, string startPlotId)
    {
        this.userId = userId;
        this.characterId = characterId;
        this.characterName = characterName;
        this.iconId = iconId;
        this.money = startMoney;
        this.currentPlotId = startPlotId;
        this.state = PlayerState.Normal;
        this.jailTurns = 0;
        
        ownedPlotIds.Clear();
        items.Clear();
    }

    /// <summary>
    /// 移动到指定地块
    /// </summary>
    public virtual void MoveToPlot(string plotId)
    {
        currentPlotId = plotId;
        OnMoved?.Invoke(plotId);
    }

    /// <summary>
    /// 增加金钱
    /// </summary>
    public virtual void AddMoney(int amount)
    {
        if (amount <= 0) return;
        
        money += amount;
        OnMoneyChanged?.Invoke(money);
    }

    /// <summary>
    /// 减少金钱
    /// </summary>
    /// <returns>是否成功减少（破产时返回false）</returns>
    public virtual bool SubtractMoney(int amount)
    {
        if (amount <= 0) return true;
        
        money -= amount;
        OnMoneyChanged?.Invoke(money);
        
        // 检查是否破产
        if (money < 0)
        {
            SetState(PlayerState.Bankrupt);
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// 设置玩家状态
    /// </summary>
    public virtual void SetState(PlayerState newState)
    {
        if (state == newState) return;
        
        state = newState;
        OnStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// 进入监狱
    /// </summary>
    public virtual void EnterJail(int turns)
    {
        SetState(PlayerState.InJail);
        jailTurns = turns;
    }

    /// <summary>
    /// 减少监狱回合数
    /// </summary>
    public virtual void ReduceJailTurn()
    {
        if (state == PlayerState.InJail)
        {
            jailTurns--;
            if (jailTurns <= 0)
            {
                SetState(PlayerState.Normal);
                jailTurns = 0;
            }
        }
    }

    /// <summary>
    /// 购买地块
    /// </summary>
    public virtual bool PurchasePlot(int plotId, int price)
    {
        if (money >= price && !ownedPlotIds.Contains(plotId))
        {
            if (SubtractMoney(price))
            {
                ownedPlotIds.Add(plotId);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 出售地块
    /// </summary>
    public virtual void SellPlot(int plotId, int price)
    {
        if (ownedPlotIds.Contains(plotId))
        {
            ownedPlotIds.Remove(plotId);
            AddMoney(price);
        }
    }

    /// <summary>
    /// 检查是否拥有指定地块
    /// </summary>
    public virtual bool OwnsPlot(int plotId)
    {
        return ownedPlotIds.Contains(plotId);
    }

    /// <summary>
    /// 添加道具
    /// </summary>
    public virtual void AddItem(Item item)
    {
        var existingItem = items.Find(i => i.itemId == item.itemId);
        if (existingItem != null)
        {
            existingItem.quantity += item.quantity;
        }
        else
        {
            items.Add(new Item
            {
                itemId = item.itemId,
                itemName = item.itemName,
                quantity = item.quantity,
                description = item.description,
                isUsable = item.isUsable
            });
        }
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    public virtual bool UseItem(string itemId)
    {
        var item = items.Find(i => i.itemId == itemId && i.isUsable);
        if (item != null)
        {
            item.quantity--;
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移除道具
    /// </summary>
    public virtual void RemoveItem(string itemId, int quantity = 1)
    {
        var item = items.Find(i => i.itemId == itemId);
        if (item != null)
        {
            item.quantity -= quantity;
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }
        }
    }

    /// <summary>
    /// 检查是否拥有指定道具
    /// </summary>
    public virtual bool HasItem(string itemId, int requiredQuantity = 1)
    {
        var item = items.Find(i => i.itemId == itemId);
        return item != null && item.quantity >= requiredQuantity;
    }

    /// <summary>
    /// 开始玩家回合（抽象方法，子类必须实现）
    /// </summary>
    public abstract void StartTurn();

    /// <summary>
    /// 结束玩家回合（抽象方法，子类必须实现）
    /// </summary>
    public abstract void EndTurn();
}
    