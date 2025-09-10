using System;
using System.Collections.Generic;
using Battle.Grid;
using Core.Enum;
using Core.Mgr;
using Core.Plugin;
using Core.Table;
using Core.Utils;
using UnityEngine;

namespace Battle.Manager
{
    public struct GridEventObject
    {
        public GridEventEnum eventType;
        public int gridId;
    }

    /// <summary>
    /// 只负责格子数据的加载、存储、查找（如GridModel的管理）。
    /// 不处理UI和业务逻辑。
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        private GridViewModel gridViewModel;
        public GridViewModel GridViewModel { get { return gridViewModel; } }
        public GameObject gridContainer;
        public GameObject gridUIContainer;
        public GameObject gridLineContainer;
        private int stationDestGridID = 1001; // 默认初始目标站点
        public int StationDestGridID { get { return stationDestGridID; } }
        private readonly Dictionary<int, GridModel> modelMap = new();
        public Dictionary<int, GridModel> ModelMap { get { return modelMap; } }
        public event Action OnUpdateStationDest;
        public static GridManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void Start()
        {
            Init();
        }

        /// <summary>
        /// 初始化格子管理器，加载格子配置并生成格子、UI和连线
        /// </summary>
        public void Init()
        {
            gridViewModel = new GridViewModel(this);
            gridViewModel.onGridEvent += OnHandlerEvent;
            ShowGrid();
        }

        private void ShowGrid()
        {
            CreateGridItem();
            ShowGridLines();
        }

        private void CreateGridItem()
        {
            GameObject gridCell = CreateManager.CreatePrefab("GridCell");
            if (!gridCell)
            {
                Debug.LogError("未能创建网格单元");
                return;
            }
            // 创建格子
            Dictionary<int, GridCfgTable> gridCfgMap = ConfigManager.LoadConfig<GridCfgTable>("GridCfg");
            Dictionary<int, GameObject> gridItemMap = new();
            foreach (var kv in gridCfgMap)
            {
                GridCfgTable cfgData = kv.Value;
                GridModel model = new(cfgData);
                modelMap.Add(model.GridId, model);
                var grid = Instantiate(gridCell, gridContainer.transform, false);
                grid.name = $"Grid_{model.GridId}";
                GridItem gridItem = grid.GetComponent<GridItem>();
                gridItem.SetData(model.GridId, gridViewModel);
                gridItemMap[model.GridId] = grid;
            }
            CreateGridUIItem(gridItemMap);
            PathFinder.Initialize(gridCfgMap);
        }

        private void CreateGridUIItem(Dictionary<int, GameObject> gridItemMap)
        {
            GameObject gridUI = CreateManager.CreatePrefab("GridUI");
            if (!gridUI)
            {
                Debug.LogError("未能创建网格单元UI");
                return;
            }
            ///创建格子UI
            foreach (var kv in modelMap)
            {
                GridModel model = kv.Value;
                var cellUIItem = Instantiate(gridUI, gridUIContainer.transform, false);
                cellUIItem.name = $"GridUI_{model.GridId}";
                var followUIComp = cellUIItem.GetComponent<FollowUI>();
                followUIComp.targetCube = gridItemMap[model.GridId].transform;
                var gridUIComp = cellUIItem.GetComponent<GridUI>();
                CityCfgTable cityConfig = ConfigManager.GetConfig<CityCfgTable>(model.CityId);
                string cityName = cityConfig.name;
                gridUIComp.SetCityName(cityName);
            }
        }

        /// <summary>
        /// 绘制所有格子的连接线
        /// </summary>
        private void ShowGridLines()
        {
            GameObject lineObject = CreateManager.CreateLineRenderer();
            foreach (var kv in modelMap)
            {
                int curGridID = kv.Key;
                var model = kv.Value;
                foreach (var connectedGridId in model.ConnectedGrids)
                {
                    if (gridViewModel.GetModel(connectedGridId) is GridModel linkedModel)
                    {
                        if (connectedGridId > curGridID)
                        {
                            var name = $"Line_{model.GridId}_{connectedGridId}";
                            if (gridLineContainer.transform.Find(name) != null)
                            {
                                Debug.LogWarning($"线条 {name} 已存在，跳过创建重复线条");
                                continue;
                            }
                            GameObject curLineObject = Instantiate(lineObject, gridLineContainer.transform, false);
                            curLineObject.name = name;
                            curLineObject.transform.position = new Vector3(model.Coordinates.x, 0.0f, model.Coordinates.z);
                            curLineObject.GetComponent<LineRenderer>().SetPosition(0, new Vector3(model.Coordinates.x, 0.01f, model.Coordinates.z));
                            curLineObject.GetComponent<LineRenderer>().SetPosition(1, new Vector3(linkedModel.Coordinates.x, 0.01f, linkedModel.Coordinates.z));
                        }
                    }
                }
            }
            Destroy(lineObject);
        }

        /// <summary>
        /// 随机选择下一个目标站点
        /// </summary>
        /// <returns></returns>
        public void RandomNextStationDest()
        {
            var keys = new List<int>(modelMap.Keys);
            keys.Remove(1001);
            if (keys.Count == 0) return;
            int idx = UnityEngine.Random.Range(0, keys.Count);
            stationDestGridID = keys[idx];
            OnUpdateStationDest?.Invoke();
        }

        public void OnHandlerEvent(GridEventObject eventData)
        {
            EventManager.TriggerEvent(GameEvent.OnGridEvent, eventData);
        }
    }
}
