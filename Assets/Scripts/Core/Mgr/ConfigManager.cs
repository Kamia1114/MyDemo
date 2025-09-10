using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Core.Enum;
using Core.Table;

namespace Core.Mgr
{
    public static class ConfigManager
    {
        // 用于存储所有类型的配置字典（按类型区分）
        private static readonly Dictionary<Type, Dictionary<int, object>> allConfigs = new();

        /// <summary>
        /// 永久配置集合
        /// </summary>
        public static readonly HashSet<string> PermanentConfigs = new HashSet<string>
        {
            // "HeroCfg",
            "GameCfg",
        };
        /// <summary>
        /// 主场景需要预加载配置
        /// </summary>
        public static readonly HashSet<string> MainSceneConfigs = new HashSet<string>
        {
            // "CommonCfg",
        };
        /// <summary>
        /// 战斗场景需要预加载配置
        /// </summary>
        public static readonly HashSet<string> BattleSceneConfigs = new HashSet<string>
        {
            // 战斗场景需要的配置
            "GridCfg",
            "CharacterCfg",
            "CityCfg",
            "CompanyCfg",
            "CardCfg",
        };

        public static async Task Init()
        {
            // 这里可以预加载一些必要的配置
            foreach (var configName in PermanentConfigs)
            {
                Type type = GetTypeByClassName(configName);
                if (type != null)
                {
                    var method = typeof(ConfigManager).GetMethod("LoadConfigAsync", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var genericMethod = method.MakeGenericMethod(type);
                    var task = (Task)genericMethod.Invoke(null, new object[] { configName });
                    await task;
                }
            }
        }

        /// <summary>
        /// 切换场景时加载和卸载配置
        /// </summary>
        /// <param name="nowSceneState"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void ChangeScene(SceneState nowSceneState)
        {
            // 1. 获取需要加载和卸载的配置名集合
            HashSet<string> loadConfigNames, unloadConfigNames;
            switch (nowSceneState)
            {
                case SceneState.Main:
                    loadConfigNames = MainSceneConfigs;
                    unloadConfigNames = BattleSceneConfigs;
                    break;
                case SceneState.Battle:
                    loadConfigNames = BattleSceneConfigs;
                    unloadConfigNames = MainSceneConfigs;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nowSceneState), nowSceneState, null);
            }

            // 2. 加载需要的配置
            foreach (var configName in loadConfigNames)
            {
                Type type = GetTypeByClassName(configName);
                if (type != null && !allConfigs.ContainsKey(type))
                {
                    var method = typeof(ConfigManager).GetMethod("LoadConfigAsync", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var genericMethod = method.MakeGenericMethod(type);
                    genericMethod.Invoke(null, new object[] { configName });
                }
            }

            // 3. 卸载不需要的配置（排除常驻配置）
            foreach (var configName in unloadConfigNames)
            {
                Type type = GetTypeByClassName(configName);
                if (type != null && allConfigs.ContainsKey(type))
                {
                    var method = typeof(ConfigManager).GetMethod("UnloadConfig", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var genericMethod = method.MakeGenericMethod(type);
                    genericMethod.Invoke(null, null);
                }
            }
        }

        /// <summary>
        /// 通用配置加载方法
        /// </summary>
        /// <typeparam name="T">配置数据类型，需包含int id字段</typeparam>
        /// <param name="configName">配置文件名（如"GrideCfg.json"）</param>
        /// <returns>以id为key的Dictionary</returns>
        public static Dictionary<int, T> LoadConfig<T>(string configName) where T : class
        {
            // 优先从缓存获取
            if (allConfigs.TryGetValue(typeof(T), out var cachedObjDict))
            {
                var cachedDict = new Dictionary<int, T>();
                foreach (var kv in cachedObjDict)
                {
                    if (kv.Value is T t)
                        cachedDict[kv.Key] = t;
                }
                return cachedDict;
            }

            var dict = new Dictionary<int, T>();
            // 使用Resources.Load加载配置
            string resourcePath = $"Configs/{configName}";
            TextAsset configAsset = Resources.Load<TextAsset>(resourcePath);
            if (configAsset == null)
            {
                Debug.LogError($"配置文件未找到: Resources/{resourcePath}");
                return dict;
            }
            string json = configAsset.text;

            // 兼容直接为数组的json内容，自动包装为对象
            if (json.TrimStart().StartsWith("["))
            {
                json = "{\"items\":" + json + "}";
            }

            // JsonUtility 只支持对象或对象数组，这里用包装类
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            if (wrapper != null && wrapper.items != null)
            {
                foreach (var item in wrapper.items)
                {
                    var idField = typeof(T).GetField("ID");
                    if (idField != null)
                    {
                        int id = (int)idField.GetValue(item);
                        dict[id] = item;
                    }
                    else
                    {
                        Debug.LogError($"类型{typeof(T).Name}未找到id字段");
                    }
                }
            }
            else
            {
                Debug.LogError($"配置文件解析失败或内容为空: {resourcePath}");
            }
            Resources.UnloadAsset(configAsset);

            // 缓存到 allConfigs 以便通过 GetConfig 获取
            var objDict = new Dictionary<int, object>();
            foreach (var kv in dict)
            {
                objDict[kv.Key] = kv.Value;
            }
            allConfigs[typeof(T)] = objDict;
            return dict;
        }

        /// <summary>
        /// 通用异步配置加载方法
        /// </summary>
        public static async Task<Dictionary<int, T>> LoadConfigAsync<T>(string configName) where T : class
        {
            // 优先从缓存获取
            if (allConfigs.TryGetValue(typeof(T), out var cachedObjDict))
            {
                var cachedDict = new Dictionary<int, T>();
                foreach (var kv in cachedObjDict)
                {
                    if (kv.Value is T t)
                        cachedDict[kv.Key] = t;
                }
                return cachedDict;
            }

            var dict = new Dictionary<int, T>();
            string resourcePath = $"Configs/{configName}";
            ResourceRequest req = Resources.LoadAsync<TextAsset>(resourcePath);
            while (!req.isDone)
            {
                await Task.Yield();
            }
            TextAsset configAsset = req.asset as TextAsset;
            if (configAsset == null)
            {
                Debug.LogError($"配置文件未找到: Resources/{resourcePath}");
                return dict;
            }
            string json = configAsset.text;

            if (json.TrimStart().StartsWith("["))
            {
                json = "{\"items\":" + json + "}";
            }

            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            if (wrapper != null && wrapper.items != null)
            {
                foreach (var item in wrapper.items)
                {
                    var idField = typeof(T).GetField("ID");
                    if (idField != null)
                    {
                        int id = (int)idField.GetValue(item);
                        dict[id] = item;
                    }
                    else
                    {
                        Debug.LogError($"类型{typeof(T).Name}未找到ID字段");
                    }
                }
            }
            else
            {
                Debug.LogError($"配置文件解析失败或内容为空: {resourcePath}");
            }
            Resources.UnloadAsset(configAsset);

            var objDict = new Dictionary<int, object>();
            foreach (var kv in dict)
            {
                objDict[kv.Key] = kv.Value;
            }
            allConfigs[typeof(T)] = objDict;
            return dict;
        }

        /// <summary>
        /// 通过id获取配置
        /// </summary>
        /// <typeparam name="T">配置数据类型</typeparam>
        /// <param name="id">配置id</param>
        /// <returns>配置数据</returns>
        public static T GetConfig<T>(int id) where T : class
        {
            string configName = typeof(T).Name.Replace("Table", "");
            if (allConfigs.TryGetValue(typeof(T), out var configDict))
            {
                if (configDict.TryGetValue(id, out var cachedConfig))
                {
                    return cachedConfig as T;
                }
            }
            return LoadConfig<T>(configName).TryGetValue(id, out var config) ? config : null;
        }

        public static T UnloadConfig<T>() where T : class
        {
            if (allConfigs.ContainsKey(typeof(T)))
            {
                allConfigs.Remove(typeof(T));
            }
            return null;
        }

        /// <summary>
        /// 根据类名字符串获取Type
        /// </summary>
        public static Type GetTypeByClassName(string className)
        {
            // 先尝试全局查找
            var type = Type.GetType("Core.Table." + className + "Table");
            if (type != null)
                return type;

            // 常见用法：加上命名空间
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(className);
                if (type != null)
                    return type;
            }
            Debug.LogError($"未找到类型: {className}");
            return null;
        }

        /// <summary>
        /// 获取游戏配置项
        /// </summary>
        public static string GetGameConfig(string keyName)
        {
            var gameCfgs = LoadConfig<GameCfgTable>("GameCfg");
            foreach (var cfg in gameCfgs.Values)
            {
                if (cfg.key == keyName)
                {
                    return cfg.value;
                }
            }
            Debug.LogError($"未找到游戏配置项: {keyName}");
            return null;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public List<T> items;
        }
    }
}