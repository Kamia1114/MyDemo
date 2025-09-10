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
		// 卡牌种类
		public CardTypeEnum type;
		// 效果类型
		public UseTypeEnum useType;
		// 卡牌名称
		public string name;
		// 效果描述
		public string desc;
		// 使用后是否可以掷骰子
		public bool extraRoll;
		// 使用后文本
		public string tip;
		// 实施目标
		public TargetTypeEnum target;
		// 持续种类
		public CountTypeEnum countType;
		// 持续数量
		public List<int> count;
		// 自定义参数
		public string arg;
	}
}
