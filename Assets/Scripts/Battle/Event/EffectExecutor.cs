// using Battle.Manager;

// public static class EffectExecutor
// {
//     public static void Execute(EffectCfgTable effect, EventContext context)
//     {
//         switch (effect.type)
//         {
//             case EffectType.AddMoney:
//                 int money = effect.param[0];
//                 context.player.ChangeMoney(money);
//                 break;
//             case EffectType.MoveSteps:
//                 int steps = effect.param[0];
//                 BattleManager.Instance.MovePlayer(context.player, steps);
//                 break;
//             case EffectType.GainCard:
//                 int cardId = effect.param[0];
//                 context.player.AddCard(cardId);
//                 break;
//                 // 扩展其他效果
//         }
//     }
// }

// public class EffectCfgTable
// {
//     public EffectType type;
//     public int[] param;
// }

// public enum EffectType
// {
//     AddMoney,    // 增加金钱
//     MoveSteps,   // 移动步数
//     GainCard,    // 获得卡牌
//     // 扩展其他效果类型
// }