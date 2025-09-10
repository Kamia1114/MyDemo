// 持续性效果状态
public class PersistentEffectState : ICardEventState
{
    private int _duration; // 持续回合数
    private int _remainingTurns; // 剩余回合数

    public PersistentEffectState(int duration)
    {
        _duration = duration;
        _remainingTurns = duration;
    }

    public void Enter(BaseEvent cardEvent)
    {
        // 初始化持续性效果
        StartPersistentEffect(cardEvent);
    }

    public void Update(BaseEvent cardEvent)
    {
        // 每回合更新持续性效果
        UpdatePersistentEffect(cardEvent);
        
        _remainingTurns--;
        if (_remainingTurns <= 0)
        {
            // 持续时间结束，切换到结束状态
            cardEvent.ChangeState(new EndEffectState());
        }
    }

    public void Exit(BaseEvent cardEvent)
    {
        // 结束持续性效果时的清理
    }

    private void StartPersistentEffect(BaseEvent cardEvent)
    {
        // 持续性效果开始逻辑
    }

    private void UpdatePersistentEffect(BaseEvent cardEvent)
    {
        // 每回合执行的持续性效果
    }
}
