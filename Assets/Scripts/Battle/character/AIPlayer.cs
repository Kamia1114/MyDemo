using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI难度等级
/// </summary>
public enum AIDifficulty
{
    Easy,      // 简单：决策保守，倾向于安全操作
    Medium,    // 中等：平衡发展，有一定攻击性
    Hard,      // 困难：积极扩张，高风险高回报
    Expert     // 专家：最优策略，几乎不会犯错
}

/// <summary>
/// AI玩家类
/// </summary>
public class AIPlayer : BasePlayer
{
    [Header("AI设置")]
    [SerializeField] private AIDifficulty difficulty = AIDifficulty.Medium;  // AI难度
    [SerializeField] private float decisionDelay = 1.5f;                    // 决策延迟（秒），模拟思考过程

    private float decisionTimer;
    private bool isMakingDecision = false;

    /// <summary>
    /// AI难度属性
    /// </summary>
    public AIDifficulty Difficulty => difficulty;

    /// <summary>
    /// 初始化AI玩家
    /// </summary>
    public override void Initialize(string userId, string characterId, string characterName, 
                          int iconId, int startMoney, string startPlotId)
    {
        base.Initialize(userId, characterId, characterName, iconId, startMoney, startPlotId);
        // AI玩家特定的初始化逻辑
    }

    /// <summary>
    /// 开始AI玩家回合
    /// </summary>
    public override void StartTurn()
    {
        SetState(PlayerState.Normal);
        Debug.Log($"{CharacterName}（AI, {difficulty}）的回合开始");
        
        // 开始AI决策流程
        isMakingDecision = true;
        decisionTimer = 0;
    }

    /// <summary>
    /// 结束AI玩家回合
    /// </summary>
    public override void EndTurn()
    {
        SetState(PlayerState.Waiting);
        Debug.Log($"{CharacterName}（AI）的回合结束");
        isMakingDecision = false;
        
        // 通知游戏管理器切换到下一个玩家
        // GameManager.Instance.NextPlayerTurn();
    }

    private void Update()
    {
        if (isMakingDecision && State == PlayerState.Normal)
        {
            decisionTimer += Time.deltaTime;
            
            // 等待决策延迟时间后执行AI逻辑
            if (decisionTimer >= decisionDelay)
            {
                MakeAIDecisions();
            }
        }
    }

    /// <summary>
    /// AI决策核心逻辑
    /// </summary>
    private void MakeAIDecisions()
    {
        // 根据当前状态决定执行什么操作
        switch (State)
        {
            case PlayerState.Normal:
                // 正常状态下先掷骰子
                RollDice();
                break;
            case PlayerState.RollingDice:
                // 掷完骰子后的逻辑由GameManager回调处理
                break;
            // 可以处理其他状态的决策逻辑
        }
    }

    /// <summary>
    /// AI掷骰子
    /// </summary>
    private void RollDice()
    {
        SetState(PlayerState.RollingDice);
        // GameManager.Instance.RollDice(this);
    }

    /// <summary>
    /// AI到达新地块后的决策
    /// </summary>
    public void OnLandOnPlot(Plot plot)
    {
        // 根据地块类型和AI难度做出不同决策
        switch (plot.PlotType)
        {
            case PlotType.City:
                DecideToPurchaseCity((CityPlot)plot);
                break;
            case PlotType.Chance:
                // 抽卡地块直接执行
                plot.OnPlayerEnter(this);
                break;
            // 处理其他类型地块的决策
            default:
                plot.OnPlayerEnter(this);
                break;
        }
    }

    /// <summary>
    /// 决定是否购买城市地块
    /// </summary>
    private void DecideToPurchaseCity(CityPlot city)
    {
        // 已经拥有则不处理
        if (OwnsPlot(city.PlotId)) return;
        
        // 其他人拥有则支付租金
        if (city.OwnerId > 0)
        {
            city.OnPlayerEnter(this);
            return;
        }
        
        // 根据难度决定是否购买
        bool shouldPurchase = false;
        int cost = city.PurchasePrice;
        
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                // 简单难度：只有钱非常充裕时才购买
                shouldPurchase = Money >= cost * 5;
                break;
            case AIDifficulty.Medium:
                // 中等难度：钱足够且有一定余量时购买
                shouldPurchase = Money >= cost * 2;
                break;
            case AIDifficulty.Hard:
                // 困难难度：只要有钱就购买
                shouldPurchase = Money >= cost;
                break;
            case AIDifficulty.Expert:
                // 专家难度：根据地块价值决定
                shouldPurchase = EvaluateCityValue(city) >= cost;
                break;
        }
        
        if (shouldPurchase)
        {
            PurchasePlot(city.PlotId, cost);
            // city.SetOwner(UserId);
            Debug.Log($"{CharacterName}（AI）购买了{city.CityName}，花费{cost}");
        }
        else
        {
            Debug.Log($"{CharacterName}（AI）决定不购买{city.CityName}");
        }
    }

    /// <summary>
    /// 评估城市地块价值（专家难度用）
    /// </summary>
    private int EvaluateCityValue(CityPlot city)
    {
        // 简单的价值评估逻辑，可以根据地块位置、租金等因素调整
        int baseValue = city.PurchasePrice;
        
        // 考虑租金收益
        baseValue += city.BaseRent * 5;
        
        // 可以添加更多评估因素
        
        return baseValue;
    }
}
    