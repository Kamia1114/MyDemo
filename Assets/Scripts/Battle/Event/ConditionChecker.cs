using Core.Enum;

public static class ConditionChecker
{
    public static bool Check(TriggerTimingEnum cond, EventContext context)
    {
        return cond switch
        {
            TriggerTimingEnum.Roll => true, // 始终满足掷骰子条件
            TriggerTimingEnum.TurnStart => true, // 始终满足回合开始条件
            TriggerTimingEnum.TurnEnd => true, // 始终满足回合结束条件
            // 扩展其他条件
            _ => false
        };
    }

    // 条件实现...
}