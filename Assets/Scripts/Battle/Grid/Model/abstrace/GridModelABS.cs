using System;
using System.Collections.Generic;
using Core.Enum;
using UnityEngine;

namespace Battle.Grid
{
    /// <summary>
    /// 地块抽象基类，所有类型的地块都继承自此
    /// </summary>
    public abstract class GridModelABS
    {
        [Header("基础属性")]
        [Tooltip("地块唯一标识")]
        protected int gridId;

        [Tooltip("地块类型")]
        protected GridTypeEnum gridType;

        [Tooltip("城市唯一标识")]
        protected int cityId;

        [Tooltip("地块在游戏板上的坐标")]
        protected Vector3 coordinates;

        [Tooltip("与之相连的其他地块")]
        protected List<int> connectedGrids;

        protected string arg;


        // // 属性访问器
        public int GridId => gridId;
        public int CityId => cityId;
        public Vector3 Coordinates => coordinates;
        public List<int> ConnectedGrids => connectedGrids;
        public string Arg => arg;

        // // 模型数据变更事件
        // public event Action OnDataChanged;

        // // 通知视图模型数据已变更
        // protected void NotifyDataChanged()
        // {
        //     OnDataChanged?.Invoke();
        // }


    }
}