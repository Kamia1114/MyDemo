using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 玩家行动状态枚举
/// </summary>
public enum PlayerAction
{
    Idle,       // 等待中
    StartDecide,     // 决策中
    Roll,       // 掷骰子中
    WaitMove,   // 等待移动
    Moving,     // 移动中
    Arrived,    // 到达目的地
    EndDecide,      // 事件处理
}

/// <summary>
/// 道具类
/// </summary>
// [Serializable]
public class Card
{
    public int id;             // 道具唯一ID
    public int cardId;       // 卡牌配置ID
    public int times;     // 剩余次数
}

public class Stock
{
    public int id;             // 股票唯一ID
    public int money;       // 股票价格
    public int ratio;     // 分红
    public bool isOwned;  // 是否被拥有
    public int cityId;    // 所在城市ID
    public int ownerId;   // 拥有者ID
}

/// <summary>
/// 玩家资产类
/// </summary>
/// [Serializable]
public class Property
{
    public int Money; // 金钱
    public List<Card> Cards; // 拥有的卡牌
    public List<Stock> OwnedCompanys; // 拥有的公司股票

    public Property()
    {
        Money = 0;
        Cards = new List<Card>();
        OwnedCompanys = new List<Stock>();
    }

    public void Reset()
    {
        Money = 0;
        Cards.Clear();
        OwnedCompanys.Clear();
    }
}

/// <summary>
/// 玩家抽象基类
/// </summary>
public abstract class PlayerModelABS
{
    [Header("玩家基本信息")]
    protected int index;          // 玩家序号(投骰子的顺序)
    protected int userId;          // 用户唯一ID
    protected int characterId;     // 角色ID
    protected string characterName;   // 角色名称
    protected string carRes;        // 车辆资源
    protected string icon;             // 角色图标
    protected int currentGridId;   // 当前所在地块ID
    protected bool isTurn;         // 是否轮到该玩家
    protected bool isAI;           // 是否为AI玩家
    protected List<int> diceList;    // 角色骰子列表
    protected int nextGridId;      // 本次移动目标格子ID
    protected List<int> movedPath; // 本次移动目标格子列表
    protected List<int> movePath;      // 本次将移动路径
    protected int remainSteps;         // 剩余移动步数
    [Header("资产信息")]
    protected Property property; // 玩家资产
    [Header("游戏状态信息")]
    protected PlayerAction playerAction;      // 行动状态

    #region 属性访问器
    public int Index => index;
    public int UserId => userId;
    public int CharacterId => characterId;
    public string CharacterName => characterName;
    public string CarRes => carRes;
    public string Icon => icon;
    public bool IsAI => isAI;
    public List<int> DiceList => diceList;
    public Property PlayerProperty => property;

    #endregion
}
    