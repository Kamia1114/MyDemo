// 主动卡牌（需玩家手动触发）
using System;
using Battle.Manager;
using Core.Enum;

namespace Battle.Card
{
    public class ActiveCard : IEventListener
    {
        private readonly CardModel model;
        public CardModel Model => model;

        public ActiveCard(CardModel model)
        {
            this.model = model;
            if (model.useType != UseTypeEnum.Active)
            {
                throw new ArgumentException("CardModel useType must be Active for ActiveCard");
            }
        }

        // 实现 IEventListener 接口的 CheckConditions 方法
        public bool CheckConditions(EventContext context)
        {
            // 根据实际需求检查条件，主动默认返回 true
            return true;
        }

        public void ExecuteEffects(EventContext context)
        {
            if (SkillManager.Instance.UseSkill(model.skillID, context))
            {
                // 使用成功，减少剩余次数
                Used(context);
            }
            else
            {
                // 使用失败，可选处理
                Console.WriteLine("ActiveCard: Skill use failed.");
            }
        }

        public void Used(EventContext context)
        {
            // 使用成功，减少剩余次数
            model.remainingTimes--;
            if (model.remainingTimes <= 0)
            {
                // 移除技能（如果是持续性技能）
                context.caster.Skills.RemoveAll(s => s.SkillId == model.skillID);
                // 移除卡牌
                PlayerManager.Instance.GetPlayerModel(model.playerIndex).RemoveCard(model.ID);
                // 移除事件监听
                CardManager.Instance.RemoveCardListener(model.ID, model.useType);
            }
        }
    }
}