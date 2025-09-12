// public static class ConditionChecker {
//     public static bool Check(ConditionCfg cond, EventContext context) {
//         return cond.type switch {
//             ConditionType.MoneyGreaterThan => CheckMoneyGreater(cond, context),
//             ConditionType.OnGridId => CheckOnGrid(cond, context),
//             ConditionType.HasCompany => CheckHasCompany(cond, context),
//             // 扩展其他条件
//             _ => false
//         };
//     }

//     private static bool CheckMoneyGreater(ConditionCfg cond, EventContext context) {
//         int threshold = cond.param[0];
//         return context.player.GetMoney() > threshold;
//     }

//     private static bool CheckOnGrid(ConditionCfg cond, EventContext context) {
//         int targetGridId = cond.param[0];
//         return context.currentGridId == targetGridId;
//     }

//     // 其他条件实现...
// }