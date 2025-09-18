using System;
using System.Collections.Generic;
using Core.Enum;
namespace Core.Table
{
	[Serializable]
	public class CardCfgTable
	{
		// 唯一id
		public int ID;
		// 卡牌名称
		public string name;
		// 卡牌种类
		public CardTypeEnum type;
		// 二级交互窗口类型枚举
		public InteractionTypeEnum interactionType;
		// 价格
		public int money;
		// 卡牌品质
		public int quality;
		// 使用类型(主动或被动)
		public UseTypeEnum useType;
		// 实施目标
		public TargetTypeEnum target;
		// 持续种类
		public CountTypeEnum countType;
		// 持续数量
		public List<int> count;
		// 卡牌操作的UI样式类型
		public List<CardUITypeEnum> uiType;
		// 使用条件
		public List<int> useCondition;
		// 被动卡触发条件(可多选)
		public List<TriggerTimingEnum> triggerTimings;
		// 对应技能表id
		public int skillID;
		// 效果参数
		public List<int> param;
		// 使用后是否可以掷骰子
		public bool extraRoll;
		// 效果描述
		public string desc;
		// 使用后文本
		public string tip;
	}
}
