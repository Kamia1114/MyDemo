using System;
using System.Collections.Generic;
using Core.Enum;
namespace Core.Table
{
	[Serializable]
	public class GameCfgTable
	{
		// 唯一id
		public int id;
		// 参数名
		public string key;
		// 参数值
		public string value;
		// 描述
		public string des;
	}
}
