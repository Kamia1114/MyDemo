// 效果结束状态
public class EndEffectState : ICardEventState
{
    public void Enter(BaseEvent cardEvent)
    {
        // 执行效果结束逻辑
        CleanupEffect(cardEvent);
        cardEvent.End();
    }

    public void Update(BaseEvent cardEvent)
    {
    }

    public void Exit(BaseEvent cardEvent)
    {
    }

    private void CleanupEffect(BaseEvent cardEvent)
    {
        // 清理效果残留
    }
}