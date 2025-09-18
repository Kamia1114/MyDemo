// 卡牌事件管理器（负责管理所有事件的生命周期）
using System;
using System.Collections.Generic;
using Battle.Card;
using Core.Enum;
using Core.Mgr;
using Core.Table;
using UnityEngine;

namespace Battle.Manager
{
    /// <summary>
    /// 
    /// </summary>
    public class CardManager
    {
        private static CardManager instance;
        public static CardManager Instance
        {
            get
            {
                instance ??= new CardManager();
                return instance;
            }
        }

        private readonly Dictionary<int, IEventListener> activeCards = new();
        public Dictionary<int, IEventListener> ActiveCards => activeCards;
        private readonly Dictionary<int, IEventListener> passiveCards = new();
        public Dictionary<int, IEventListener> PassiveCards => passiveCards;

        public void RegisterActiveCardListener(int id, IEventListener card)
        {
            if (!activeCards.ContainsKey(id))
            {
                activeCards.Add(id, card);
            }
        }

        public void UnregisterActiveCardListener(int id)
        {
            if (activeCards.ContainsKey(id))
            {
                activeCards.Remove(id);
            }
        }

        public void RegisterPassiveCardListener(int id, IEventListener card)
        {
            if (!passiveCards.ContainsKey(id))
            {
                passiveCards.Add(id, card);
            }
        }

        public void UnregisterPassiveCardListener(int id)
        {
            if (passiveCards.ContainsKey(id))
            {
                passiveCards.Remove(id);
            }
        }

        /// <summary>
        /// 创建卡牌数据模型，并根据类型注册到相应的事件系统
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public CardModel CreateCardModel(int cardId, int playerIndex)
        {
            var cardCfg = ConfigManager.GetConfig<CardCfgTable>(cardId);
            var card = new CardModel
            {
                ID = Guid.NewGuid().GetHashCode(),
                cardID = cardCfg.ID,
                playerIndex = playerIndex,
                skillID = cardCfg.skillID,
                remainingTimes = cardCfg.count.Count == 1 ? cardCfg.count[0] : UnityEngine.Random.Range(cardCfg.count[0], cardCfg.count[1] + 1),
                useType = cardCfg.useType,
                parameters = cardCfg.param
            };
            if (card.useType == UseTypeEnum.Active)
            {
                RegisterActiveCardListener(card.ID, new ActiveCard(card));
            }
            else if (card.useType == UseTypeEnum.Passive)
            {
                // 注册被动卡牌的触发时机
                var passiveCard = new PassiveCard(card);
                cardCfg.triggerTimings.ForEach(triggerTiming =>
                    EventCenter.Instance.Register(triggerTiming, passiveCard));
                RegisterPassiveCardListener(card.ID, passiveCard);
            }
            else
            {
                Debug.LogWarning($"未知的卡牌使用类型: {card.useType}");
            }
            return card;
        }

        /// <summary>
        /// 使用主动卡牌
        /// </summary>
        public void UseCard(int id, EventContext eventContext)
        {
            if (activeCards.TryGetValue(id, out var cardListener))
            {
                // 确保是主动卡牌
                if (cardListener is ActiveCard activeCardListener)
                {
                    if (activeCardListener.CheckConditions(eventContext))
                    {
                        activeCardListener.ExecuteEffects(eventContext);
                    }
                    else
                    {
                        Debug.LogWarning($"主动卡牌使用条件不满足，ID {id}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"尝试使用未注册的主动卡牌，ID {id}");
            }
        }

        public void RemoveCardListener(int id, UseTypeEnum useType)
        {
            if (useType == UseTypeEnum.Active)
            {
                UnregisterActiveCardListener(id);
            }
            else if (useType == UseTypeEnum.Passive)
            {
                if (passiveCards.TryGetValue(id, out var cardListener) && cardListener is PassiveCard passiveCard)
                {
                    var cardCfg = ConfigManager.GetConfig<CardCfgTable>(passiveCard.Model.cardID);
                    // 取消注册被动卡牌的触发时机
                    cardCfg.triggerTimings.ForEach(triggerTiming =>
                        EventCenter.Instance.Unregister(triggerTiming, passiveCard));
                    UnregisterPassiveCardListener(id);
                }
            }
            else
            {
                Debug.LogWarning($"尝试移除未知类型的卡牌，ID {id}");
            }
        }
    }
}