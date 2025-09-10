public static class GameEvent
{
    public const string OnGridEvent = "OnGridEvent"; // 玩家点击格子事件
    public const string OnPlayerEvent = "OnPlayerEvent"; // 玩家事件
}

public enum EventEnum
{
    OnGridClicked, // 玩家点击格子
    OnPlayerArrived, // 玩家到达目的地
    OnPlayerMoveStart, // 玩家开始移动
    OnPlayerMoveEnd, // 玩家结束移动
}
