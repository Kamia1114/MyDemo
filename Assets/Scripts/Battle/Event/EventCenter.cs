using System.Collections.Generic;
using System.Linq;
using Battle.Card;
using Core.Enum;
using Unity.VisualScripting;

using UnityEngine;

public class EventCenter
{
    // 单例实例
    private static EventCenter instance;
    public static EventCenter Instance
    {
        get
        {
            instance ??= new EventCenter();
            return instance;
        }
    }

    // 事件监听字典：Key=触发时机，Value=该时机下的所有监听者（卡牌/系统）
    private readonly Dictionary<TriggerTimingEnum, List<IEventListener>> listeners = new();

    // 注册事件监听
    public void Register(TriggerTimingEnum timing, IEventListener listener)
    {
        if (!listeners.ContainsKey(timing))
        {
            listeners[timing] = new List<IEventListener>();
        }
        listeners[timing].Add(listener);
    }

    // 移除事件监听
    public void Unregister(TriggerTimingEnum timing, IEventListener listener)
    {
        if (listeners.TryGetValue(timing, out var list))
        {
            list.Remove(listener);
        }
    }

    // 触发事件（带上下文参数）
    public void Trigger(TriggerTimingEnum timing, EventContext context = null)
    {
        if (listeners.TryGetValue(timing, out var list))
        {
            // 复制列表避免触发中移除元素导致异常
            foreach (var listener in list.ToList()) // 遍历副本
            {
                // 检查所有条件是否满足
                if (listener.CheckConditions(context))
                {
                    listener.ExecuteEffects(context);
                }
            }
        }
    }
}

// 事件上下文（传递触发时的关键数据）
public class EventContext
{
    public PlayerModel caster;   // 触发事件的玩家
    public PlayerModel target;   // 事件目标玩家（可选）
    public List<int> parameters;    // 配置参数
    public List<int> tempData;     // 临时数据（可选）
    // public int currentGridId;    // 当前格子ID
    // public int moneyChange;      // 金钱变化量（可选）
    // 其他需要传递的临时数据
}