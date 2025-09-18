using System.Collections.Generic;

public static class GameEvent
{
    public const string OnBattleEvent = "OnBattleEvent"; // 游戏中事件
}

public class EventStruct
{
    public EventEnum eventType;
    public object args;

    public EventStruct(EventEnum type, params object[] parameters)
    {
        eventType = type;
        args = parameters;
    }
}


public enum EventEnum
{
    OnGridClicked, // 玩家点击格子
    OnPlayerArrived, // 玩家到达目的地
    OnPlayerMoveStart, // 玩家开始移动
    OnPlayerMoveEnd, // 玩家结束移动
    OnPlayerLuck, // 玩家幸运状态事件
    OnShowMessage, // 显示通用消息
}
