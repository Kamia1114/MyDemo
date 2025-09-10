// 卡牌事件管理器（负责管理所有事件的生命周期）
using System.Collections.Generic;

public class CardEventManager
{
    private List<BaseEvent> _activeEvents = new List<BaseEvent>();

    public void AddEvent(BaseEvent newEvent)
    {
        _activeEvents.Add(newEvent);
    }

    // 每回合更新所有事件
    public void UpdateEvents()
    {
        foreach (var evt in _activeEvents)
        {
            evt.Update();
            if (!evt.IsActive)
            {
                _activeEvents.Remove(evt);
            }
        }
    }
}