// 一次性效果状态
public class OnceEffectState : ICardEventState
{
    public void Enter(BaseEvent cardEvent)
    {
        // 执行一次性效果
        ExecuteInstantEffect(cardEvent);
        // 立即结束事件
        cardEvent.End();
    }

    public void Update(BaseEvent cardEvent)
    {
        // 一次性效果不需要更新
    }

    public void Exit(BaseEvent cardEvent)
    {
        // 清理资源
    }

    private void ExecuteInstantEffect(BaseEvent cardEvent)
    {
        // 实现一次性效果逻辑
        if (cardEvent is MoveEvent moveEvent)
        {
            // 处理移动事件的一次性效果
        }
        // 可以扩展其他类型的一次性效果
    }
}