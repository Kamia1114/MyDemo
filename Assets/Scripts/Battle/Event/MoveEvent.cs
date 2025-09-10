// 移动事件实现（继承自BaseEvent）
public class MoveEvent : BaseEvent
{
    public MoveEvent(int cardId) : base(cardId)
    {
        // 根据卡牌类型设置初始状态
        // 示例：设置为一次性效果
        ChangeState(new OnceEffectState());
    }
    
    public override void Update()
    {
        base.Update();
        // 可以添加移动事件特有的更新逻辑
    }
    
    // 移动事件特有的方法
    public void ExecuteMove()
    {
        // 实现移动逻辑
    }
}