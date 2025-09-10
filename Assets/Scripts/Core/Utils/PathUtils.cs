// 寻路结果
using System.Collections.Generic;
using System.Linq;
using Core.Table;

public class PathResult
{
    public List<int> StationIds { get; set; } // 路径站点ID列表
    public List<string> StationNames { get; set; } // 路径站点名称列表
    public int StepCount { get; set; }        // 步数（经过的站点数-1）
    public bool Found { get; set; }           // 是否找到路径
}

namespace Core.Utils
{
    /// <summary>
    /// 寻路工具类，使用BFS算法寻找最短路径
    /// </summary>
    public static class PathFinder
    {
        public static Dictionary<int, GridCfgTable> _stations; // 所有站点

        public static void Initialize(Dictionary<int, GridCfgTable> stations)
        {
            _stations = stations;
        }

        // 使用BFS寻找步数最少的路径
        public static PathResult FindShortestPath(int startId, int targetId, List<int> existingPath = null)
        {
            // 检查站点是否存在
            if (!_stations.ContainsKey(startId) || !_stations.ContainsKey(targetId))
            {
                return new PathResult { Found = false };
            }

            // 如果起点就是终点
            if (startId == targetId)
            {
                // 检查是否与已有路径重复（仅含起点/终点的情况）
                if (existingPath != null && existingPath.Count == 1 && existingPath[0] == startId)
                {
                    return new PathResult { Found = false };
                }

                return new PathResult
                {
                    Found = true,
                    StepCount = 0,
                    StationIds = new List<int> { startId },
                    StationNames = new List<string> { _stations[startId].name }
                };
            }

            // 记录已访问的站点
            var visited = new HashSet<int>();
            // 记录到达每个站点的路径
            var pathTracker = new Dictionary<int, int>();
            // BFS队列
            var queue = new Queue<int>();

            // 初始化
            queue.Enqueue(startId);
            visited.Add(startId);
            pathTracker[startId] = -1; // 起点没有前置站点

            // 水波式扩散搜索
            while (queue.Count > 0)
            {
                int currentId = queue.Dequeue();

                // 遍历当前站点的所有连接站点
                foreach (int neighborId in _stations[currentId].links)
                {
                    if (!visited.Contains(neighborId))
                    {
                        // 记录路径
                        pathTracker[neighborId] = currentId;
                        visited.Add(neighborId);
                        queue.Enqueue(neighborId);

                        // 检查是否到达目标站点
                        if (neighborId == targetId)
                        {
                            // 构建路径结果
                            var result = BuildPathResult(pathTracker, startId, targetId);

                            // 检查是否与已有路径完全重复
                            if (existingPath != null && IsPathDuplicate(result.StationIds, existingPath))
                            {
                                // 如果重复则继续搜索（清除当前路径的访问记录，继续寻找下一条路径）
                                visited.Remove(neighborId);
                                pathTracker.Remove(neighborId);
                                continue;
                            }

                            return result;
                        }
                    }
                }
            }

            // 如果队列为空仍未找到目标，说明没有路径
            return new PathResult { Found = false };
        }

        // 构建路径结果
        private static PathResult BuildPathResult(Dictionary<int, int> pathTracker, int startId, int targetId)
        {
            var stationIds = new List<int>();
            int current = targetId;

            // 回溯路径
            while (current != -1)
            {
                stationIds.Add(current);
                current = pathTracker[current];
            }

            // 反转路径，从起点到终点
            stationIds.Reverse();

            // 获取站点名称
            var stationNames = stationIds.Select(id => _stations[id].name).ToList();

            return new PathResult
            {
                Found = true,
                StationIds = stationIds,
                StationNames = stationNames,
                StepCount = stationIds.Count - 1 // 步数 = 站点数 - 1
            };
        }

        // 检查两条路径是否完全重复
        private static bool IsPathDuplicate(List<int> newPath, List<int> existingPath)
        {
            // 路径长度不同则一定不重复
            if (newPath.Count != existingPath.Count)
                return false;

            // 逐个比较站点ID
            for (int i = 0; i < newPath.Count; i++)
            {
                if (newPath[i] != existingPath[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 获取起点到终点的步数
        /// </summary>
        /// <param name="startId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public static int GetStepCount(int startId, int targetId)
        {
            // 检查起点和终点是否在同一条路径上
            var path = FindShortestPath(startId, targetId);
            return path.Found ? path.StepCount : -1;
        }
    }
}