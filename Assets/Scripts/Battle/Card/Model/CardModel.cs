// 事件监听接口（被动卡牌实现）
using System.Collections.Generic;
using Core.Enum;
using Core.Mgr;
using Core.Table;

/// <summary>
/// 道具类
/// </summary>
// [Serializable]
public class CardModel
{
    public int ID;             // 道具唯一ID
    public int cardID;       // 卡牌配置ID
    public int playerIndex; // 持有者玩家序号
    public int skillID;      // 关联技能ID
    public int remainingTimes;     // 剩余次数
    public List<int> parameters; // 参数列表
    public UseTypeEnum useType; // 使用类型（主动或被动）
}