// // 事件监听接口（被动卡牌实现）
// using Core.Mgr;
// using Core.Table;

// public interface IEventListener {
//     bool CheckConditions(EventContext context); // 检查是否满足触发条件
//     void ExecuteEffects(EventContext context);  // 执行效果
// }

// // 卡牌基类
// public class Card {
//     public int id;               // 实例ID（唯一）
//     public int cardId;           // 配置表ID
//     public int remainingTimes;   // 剩余使用次数
//     public CardCfgTable Cfg => ConfigManager.GetConfig<CardCfgTable>(cardId);
// }

// // 主动卡牌（需玩家手动触发）
// public class ActiveCard : Card {
//     // 主动使用（返回是否使用成功）
//     public bool Use(PlayerModel user, EventContext context) {
//         if (remainingTimes <= 0) return false;
//         // 检查使用条件
//         if (!CheckConditions(context)) return false;
//         // 执行效果
//         ExecuteEffects(context);
//         remainingTimes--;
//         return true;
//     }

//     private bool CheckConditions(EventContext context) {
//         // 检查配置中的条件
//         return Cfg.conditions.All(cond => ConditionChecker.Check(cond, context));
//     }

//     private void ExecuteEffects(EventContext context) {
//         foreach (var effect in Cfg.effects) {
//             EffectExecutor.Execute(effect, context);
//         }
//     }
// }

// // 被动卡牌（自动监听事件）
// public class PassiveCard : Card, IEventListener {
//     public PassiveCard() {
//         // 注册到事件中心
//         foreach (var timing in Cfg.triggerTimings) {
//             EventCenter.Instance.Register(timing, this);
//         }
//     }

//     // 实现IEventListener接口
//     public bool CheckConditions(EventContext context) {
//         // 额外检查：卡牌是否还在持有中
//         if (remainingTimes <= 0) return false;
//         // 检查配置条件
//         return Cfg.conditions.All(cond => ConditionChecker.Check(cond, context));
//     }

//     public void ExecuteEffects(EventContext context) {
//         foreach (var effect in Cfg.effects) {
//             EffectExecutor.Execute(effect, context);
//         }
//         // 消耗次数（一次性被动卡）
//         if (Cfg.maxTimes > 0) {
//             remainingTimes--;
//             if (remainingTimes <= 0) {
//                 // 移除监听
//                 foreach (var timing in Cfg.triggerTimings) {
//                     EventCenter.Instance.Unregister(timing, this);
//                 }
//             }
//         }
//     }
// }