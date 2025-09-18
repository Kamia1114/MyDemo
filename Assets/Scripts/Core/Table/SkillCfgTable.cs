using System;
using System.Collections.Generic;
using Core.Enum;
namespace Core.Table
{
	[Serializable]
	public class SkillCfgTable
	{
		// 唯一id
		public int ID;
		// 技能名
		public string name;
		// 技能枚举
		public SkillTypeEnum skillType;
		// 效果描述
		public string desc;
	}
}
