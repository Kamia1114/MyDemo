
using Core.Enum;
using UnityEngine;

namespace Battle.Skill
{
    public class WealthGod : SkillBase
    {
        public override void Use(EventContext context)
        {
            var addMoney = 0;
            // 实现加骰子逻辑
            if (context.tempData[0] <= context.parameters[2])
            {
                addMoney = context.parameters[3];
                context.caster.ChangeMoney(addMoney);
            }
            else if (context.tempData[0] <= context.parameters[0])
            {
                addMoney = context.parameters[1];
                context.caster.ChangeMoney(addMoney);
            }
            EventManager.TriggerEvent(GameEvent.OnBattleEvent, new EventStruct(EventEnum.OnShowMessage, $"{context.caster.CharacterName}触发财神技能，获得{addMoney}金币") );
        }
    }
}