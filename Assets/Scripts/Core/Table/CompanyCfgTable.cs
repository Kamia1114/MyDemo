using System;
using System.Collections.Generic;
using Core.Enum;
namespace Core.Table
{
	[Serializable]
	public class CompanyCfgTable
	{
		// 唯一id
		public int ID;
		// 地名
		public string name;
		// 公司描述
		public string desc;
		// 房产类型（农业等
		public string type;
		// 股份价格(万)
		public int money;
		// 收益比例(%)
		public int ratio;
	}
}
