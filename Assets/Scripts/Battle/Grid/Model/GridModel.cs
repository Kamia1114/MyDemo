using System;
using Battle.Manager;
using Core.Enum;
using Core.Table;
using UnityEngine;

namespace Battle.Grid
{
    /// <summary>
    /// 地块数据模型类，存储地块的基本信息和状态
    /// </summary>
    public class GridModel : GridModelABS
    {
        public event Action OnDataChanged;

        public GridTypeEnum GridType
        {
            get => gridType; 
            set
            {
                if (gridType != value)
                {
                    gridType = value;
                    OnDataChanged?.Invoke();
                }
            }
        }

        public GridModel(GridCfgTable configData)
        {
            gridId = configData.ID;
            gridType = configData.type;
            cityId = configData.cfgID;
            coordinates = new Vector3(configData.coord[0] * GameConfig.GridInterval, GameConfig.GridOffsetY, configData.coord[1] * GameConfig.GridInterval);
            connectedGrids = configData.links;
            arg = configData.arg;
        }

    }
}
