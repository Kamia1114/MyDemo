using System.Collections.Generic;
using Battle.Skill;
using Core.Enum;
using Core.Mgr;
using Core.Table;

namespace Battle.Manager
{
    public class SkillManager
    {
        private readonly Dictionary<SkillTypeEnum, System.Type> skillTypeDictionary = new();

        public static SkillManager Instance { get; } = new SkillManager();

        public void Register()
        {
            // 注册技能类型
            skillTypeDictionary[SkillTypeEnum.DiceCount] = typeof(AddDice);
            skillTypeDictionary[SkillTypeEnum.WealthGod] = typeof(WealthGod);
        }

        SkillBase CreateSkill(int skillId)
        {
            var skillCfg = ConfigManager.GetConfig<SkillCfgTable>(skillId);
            if (skillTypeDictionary.TryGetValue(skillCfg.skillType, out var type))
            {
                var skill = (SkillBase)System.Activator.CreateInstance(type);
                skill.SkillId = skillId;
                return skill;
            }
            return null;
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        public bool UseSkill(int skillId, EventContext context)
        {
            // 优先从角色技能列表中查找（持续性技能[buff/debuff]）
            var skill = context.caster.Skills.Find(s => s.SkillId == skillId);
            skill ??= CreateSkill(skillId);
            if (skill != null && skill.CheckConditions(context))
            {
                skill.Use(context);
                return true;
            }
            return false;
        }
    }
}