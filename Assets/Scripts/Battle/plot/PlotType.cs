/// <summary>
/// 地块类型枚举，定义大富翁游戏中所有可能的地块类型
/// </summary>
public enum PlotType
{
    /// <summary>
    /// 起点地块，玩家回合开始或经过时可获得金钱
    /// </summary>
    Start,
    
    /// <summary>
    /// 城市地块，可购买
    /// </summary>
    City,
    
    /// <summary>
    /// 抽卡地块，玩家进入时可抽取随机卡片
    /// </summary>
    CardDraw,
    
    /// <summary>
    /// 加钱地块，玩家进入时获得一定数量金钱
    /// </summary>
    MoneyAdd,
    
    /// <summary>
    /// 扣钱地块，玩家进入时扣除一定数量金钱
    /// </summary>
    MoneyDeduct,
    
    /// <summary>
    /// 机会/命运地块，类似抽卡但可能触发特殊事件
    /// </summary>
    Chance,
    
    /// <summary>
    /// 机场地块，可快速移动到其他机场
    /// </summary>
    Airport,
    
    /// <summary>
    /// 特殊事件地块，可能触发随机事件（如地震、好运等）
    /// </summary>
    Event,
    
    /// <summary>
    /// 未知类型地块，作为默认或错误处理
    /// </summary>
    Unknown
}
    