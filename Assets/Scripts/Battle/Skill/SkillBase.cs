using Battle.Card;
using Core.Enum;

namespace Battle.Skill
{
    public abstract class SkillBase: IEventListener
    {
        public int SkillId { get; set; }
        // 技能基础类
        public abstract void Use(EventContext context);

        public virtual bool CheckConditions(EventContext context)
        {
            // 默认条件检查，子类可重写
            return true;
        }

        public virtual void ExecuteEffects(TriggerTimingEnum timing, EventContext context)
        {
            // 默认触发逻辑，子类可重写
        }
    }
}