// using System.Collections.Generic;
// using Core.Enum;
// using Unity.VisualScripting;

// using UnityEngine;

// public class EventCenter : MonoBehaviour {
//     // 单例实例
//     public static EventCenter Instance { get; private set; }

//     // 事件监听字典：Key=触发时机，Value=该时机下的所有监听者（卡牌/系统）
//     private readonly Dictionary<TriggerTiming, List<IEventListener>> listeners = new();

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     // 注册事件监听
//     public void Register(TriggerTiming timing, IEventListener listener) {
//         if (!listeners.ContainsKey(timing)) {
//             listeners[timing] = new List<IEventListener>();
//         }
//         listeners[timing].Add(listener);
//     }

//     // 移除事件监听
//     public void Unregister(TriggerTiming timing, IEventListener listener) {
//         if (listeners.TryGetValue(timing, out var list)) {
//             list.Remove(listener);
//         }
//     }

//     // 触发事件（带上下文参数）
//     public void Trigger(TriggerTiming timing, EventContext context) {
//         if (listeners.TryGetValue(timing, out var list)) {
//             // 复制列表避免触发中移除元素导致异常
//             foreach (var listener in list) {
//                 // 检查所有条件是否满足
//                 if (listener.CheckConditions(context)) {
//                     listener.ExecuteEffects(context);
//                 }
//             }
//         }
//     }
// }

// // 事件上下文（传递触发时的关键数据）
// public class EventContext {
//     public PlayerModel player;   // 触发事件的玩家
//     public int currentGridId;    // 当前格子ID
//     public int moneyChange;      // 金钱变化量（可选）
//     // 其他需要传递的临时数据
// }