using System;
using System.Collections.Generic;
using Core.Enum;
namespace Core.Table
{
	[Serializable]
	public class CharacterCfgTable
	{
		// 唯一id
		public int ID;
		// 角色名称
		public string name;
		// 角色模型
		public string model;
		// 角色骰子
		public List<int> dice;
		// 初始资金（万）
		public int money;
		// 初始卡牌
		public List<int> card;
		// 角色技能
		public int skillID;
		// 技能效果
		public string effect;
		// 播报文本
		public string text;
	}
}
