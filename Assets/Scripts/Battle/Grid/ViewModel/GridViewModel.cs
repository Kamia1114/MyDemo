using System;
using System.Collections.Generic;
using Battle.Manager;
using Core.Enum;
using UnityEngine;

namespace Battle.Grid
{
    /// <summary>
    /// 负责格子相关的业务逻辑（如格子点击、状态变更），并通知View层。
    /// </summary>
    public class GridViewModel
    {
        public Action<GridEventObject> onGridEvent;

        private GridManager gridManager;
        public GridViewModel(GridManager manager)
        {
            gridManager = manager;
        }

        public GridModel GetModel(int gridId)
        {
            if (gridManager.ModelMap.TryGetValue(gridId, out var model))
            {
                return model;
            }
            return null;
        }

        // 示例：处理格子被点击
        public void OnGridClicked(int gridId)
        {
            onGridEvent?.Invoke(new GridEventObject { gridId = gridId, eventType = GridEventEnum.OnGridClicked });
        }
    }
}