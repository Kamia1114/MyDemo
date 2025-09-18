using System;
using System.Collections.Generic;
using System.IO;
using Battle.Grid;
using Core.Enum;
using Core.Mgr;
using Core.Plugin;
using Core.Table;
using Core.Utils;
using UnityEngine;

namespace Battle.Manager
{

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
        // 终点
        private int stationDestGridID = 0;
        public int StationDestGridID { get { return stationDestGridID; } }
        private readonly Dictionary<int, GridModel> gridModelMap = new();
        private readonly Dictionary<int, GameObject> gridItemMap = new();
        private readonly Dictionary<int, GameObject> tipGridItemMap = new();
        public Dictionary<int, GridModel> ModelMap { get { return gridModelMap; } }
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
            Dictionary<int, GridCfgTable> gridCfgMap = ConfigManager.GetAllConfig<GridCfgTable>();
            foreach (var kv in gridCfgMap)
            {
                GridCfgTable cfgData = kv.Value;
                GridModel model = new(cfgData);
                gridModelMap.Add(model.GridId, model);
                var grid = Instantiate(gridCell, gridContainer.transform, false);
                grid.name = $"Grid_{model.GridId}";
                GridItem gridItem = grid.GetComponent<GridItem>();
                gridItem.SetData(model.GridId, gridViewModel);
                gridItemMap.Add(model.GridId, grid);
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
            foreach (var kv in gridModelMap)
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
            foreach (var kv in gridModelMap)
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
                            curLineObject.transform.position = new Vector3(model.Coordinates.x, GameConfig.GridOffsetY - 0.01f, model.Coordinates.z);
                            curLineObject.GetComponent<LineRenderer>().SetPosition(0, new Vector3(model.Coordinates.x, GameConfig.GridOffsetY - 0.01f, model.Coordinates.z));
                            curLineObject.GetComponent<LineRenderer>().SetPosition(1, new Vector3(linkedModel.Coordinates.x, GameConfig.GridOffsetY - 0.01f, linkedModel.Coordinates.z));
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
            int startPoint = int.Parse(ConfigManager.GetGameConfig("StartPoint"));
            var keys = new List<int>(gridModelMap.Keys);
            keys.Remove(startPoint);
            if (keys.Count == 0) return;
            int idx = UnityEngine.Random.Range(0, keys.Count);
            if (stationDestGridID != 0)
            {
                ShowOrHideHalo(gridItemMap[stationDestGridID], false);
            }
            stationDestGridID = keys[idx];
            ShowOrHideHalo(gridItemMap[stationDestGridID], true);

            OnUpdateStationDest?.Invoke();
        }

        private void ShowOrHideHalo(GameObject gridItem, bool show)
        {
            var halo = gridItem.GetComponent("Halo");
            if (halo != null)
            {
                halo.GetType().GetProperty("enabled").SetValue(halo, show);
            }
        }

        public void ShowNearGridTip(PlayerModel playerModel)
        {
            if (playerModel.CurrentGridId == stationDestGridID) return;
            var paths = PathFinder.FindAllShortestPaths(playerModel.CurrentGridId, stationDestGridID);
            var nearestGrids = new List<int>();
            foreach (var path in paths)
            {
                if (path.Found)
                {
                    Debug.Log($"path.StepCount: {path.StepCount}");
                    if (!nearestGrids.Contains(path.StationIds[1]))
                    {
                        nearestGrids.Add(path.StationIds[1]);
                    }
                }
            }
            foreach (var kv in tipGridItemMap)
            {
                Transform tip = kv.Value.transform.Find("tip");
                tip.gameObject.SetActive(false);
            }
            tipGridItemMap.Clear();
            nearestGrids.ForEach(gridId =>
            {
                if (gridItemMap.TryGetValue(gridId, out var gridItem))
                {
                    Transform tip = gridItem.transform.Find("tip");
                    tip.gameObject.SetActive(true);
                    tipGridItemMap.Add(gridId, gridItem);
                }
            });
        }

        public void HideNearGridTip()
        {
            foreach (var kv in tipGridItemMap)
            {
                Transform tip = kv.Value.transform.Find("tip");
                tip.gameObject.SetActive(false);
            }
            tipGridItemMap.Clear();
        }

        public void OnHandlerEvent(EventStruct eventData)
        {
            EventManager.TriggerEvent(GameEvent.OnBattleEvent, eventData);
        }
    }
}
