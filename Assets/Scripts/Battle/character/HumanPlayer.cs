using UnityEngine;

/// <summary>
/// 人类玩家类
/// </summary>
public class HumanPlayer : BasePlayer
{
    [Header("输入设置")]
    [SerializeField] private KeyCode rollDiceKey = KeyCode.Space;  // 掷骰子按键

    /// <summary>
    /// 初始化人类玩家
    /// </summary>
    public override void Initialize(string userId, string characterId, string characterName, 
                          int iconId, int startMoney, string startPlotId)
    {
        base.Initialize(userId, characterId, characterName, iconId, startMoney, startPlotId);
        // 可以添加人类玩家特定的初始化逻辑
    }

    /// <summary>
    /// 开始人类玩家回合
    /// </summary>
    public override void StartTurn()
    {
        SetState(PlayerState.Normal);
        Debug.Log($"{CharacterName}的回合开始，请按{rollDiceKey}键掷骰子");
        // 可以在这里显示UI提示，等待玩家输入
    }

    /// <summary>
    /// 结束人类玩家回合
    /// </summary>
    public override void EndTurn()
    {
        SetState(PlayerState.Waiting);
        Debug.Log($"{CharacterName}的回合结束");
        // 通知游戏管理器切换到下一个玩家
        GameManager.Instance.NextPlayerTurn();
    }

    // 可以添加处理玩家输入的方法
    private void Update()
    {
        if (State == PlayerState.Normal)
        {
            HandlePlayerInput();
        }
    }

    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandlePlayerInput()
    {
        // 处理掷骰子输入
        if (Input.GetKeyDown(rollDiceKey) && State != PlayerState.RollingDice)
        {
            SetState(PlayerState.RollingDice);
            GameManager.Instance.RollDice(this);
        }

        // 可以添加处理其他输入的逻辑，如使用道具、购买地块等
    }
}
    