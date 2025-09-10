using System;
using System.Collections.Generic;
using Core.Enum;
namespace Core.Table
{
	[Serializable]
	public class GridCfgTable
	{
		// 唯一id
		public int ID;
		// 格子类型
		public GridTypeEnum type;
		// 城名
		public string name;
		// 配置ID
		public int cfgID;
		// 坐标
		public List<int> coord;
		// 连接id
		public List<int> links;
		// 特殊参数
		public string arg;
	}
}
