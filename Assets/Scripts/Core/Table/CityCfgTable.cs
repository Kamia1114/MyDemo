using System;
using System.Collections.Generic;
using Core.Enum;
namespace Core.Table
{
	[Serializable]
	public class CityCfgTable
	{
		// 唯一id
		public int ID;
		// 地名
		public string name;
		// 照片图片资源
		public string pic;
		// 描述
		public string desc;
		// 包含的公司id
		public List<int> companysID;
	}
}
