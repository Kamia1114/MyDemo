// 被动卡牌（自动监听事件）
using System;
using System.Collections.Generic;
using System.Linq;
using Battle.Manager;
using Core.Enum;
using Core.Mgr;
using Core.Table;

namespace Battle.Card
{
    public class PassiveCard : IEventListener
    {

        private readonly CardModel model;
        public CardModel Model => model;

        public PassiveCard(CardModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// 实现IEventListener接口
        /// 是否所有触发条件通过（当有多个触发条件，例如金钱达到多少，有指定类型公司等）
        /// </summary>
        /// <param name="context"> 触发时传入的事件 </param>
        /// <returns></returns>
        public bool CheckConditions(EventContext context)
        {
            // 额外检查：卡牌是否还在持有中
            if (model.remainingTimes <= 0) return false;
            var Cfg = ConfigManager.GetConfig<CardCfgTable>(model.cardID);
            // 根据卡牌id，创建新的上下文，例如玩家需要多少财产才触发，需要tempData设置为当前的钱数量
            var newContext = CreateNewContext(context);
            // 检查配置条件（激活时机和限制条件应该分两个参数配置，正式版本修改）
            return Cfg.triggerTimings.All(cond => ConditionChecker.Check(cond, newContext));
        }

        public void ExecuteEffects(EventContext context)
        {
            // 根据卡牌id，创建新的上下文，例如财神卡需要tempData设置为剩余次数
            var newContext = CreateNewContext(context);
            if (SkillManager.Instance.UseSkill(model.skillID, newContext))
            {
                // 使用成功，减少剩余次数
                Used();
            }
            else
            {
                // 使用失败，可选处理
                Console.WriteLine("ActiveCard: Skill use failed.");
            }
        }

        // 创建新的事件上下文，确保caster正确
        public EventContext CreateNewContext(EventContext context)
        {
            var caster = context?.caster;
            caster ??= PlayerManager.Instance.GetPlayerModel(model.playerIndex);
            List<int> tempData = GetTempDataByCardID(model.cardID);
            return new EventContext
            {
                caster = caster,
                target = context?.target,
                parameters = model.parameters,
                tempData = tempData
            };
        }

        public void Used()
        {
            // 使用成功，减少剩余次数
            model.remainingTimes--;
            if (model.remainingTimes <= 0)
            {
                // 次数用尽，移除卡牌
                PlayerManager.Instance.GetPlayerModel(model.playerIndex).RemoveCard(model.ID);
                CardManager.Instance.RemoveCardListener(model.ID, model.useType);
            }
        }

        // 根据卡牌ID返回临时数据
        public List<int> GetTempDataByCardID(int key)
        {
            return key switch
            {
                // 财神
                30070 => new List<int> { model.remainingTimes },
                _ => new List<int> { 0 },
            };
        }
    }
}