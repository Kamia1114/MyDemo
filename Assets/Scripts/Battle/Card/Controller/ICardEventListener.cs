namespace Battle.Card
{
    // 事件监听接口（被动卡牌实现）
    public interface IEventListener
    {
        bool CheckConditions(EventContext context); // 检查是否满足触发条件
        void ExecuteEffects(EventContext context) { }  // 执行效果，默认空实现，可选
    }
}