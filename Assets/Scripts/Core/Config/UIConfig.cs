using System.Collections.Generic;

public enum UIType
{
    Main,
    Normal,
    Pop,
    Top,
    Notice,
}

public class UIConfigData
{
    public string PrefabUrl; // 预制体路径
    public UIType Layer;        // 所属层级
    public bool closeLayer; // 是否关闭同层级其他UI
    public bool IsSingleton; // 是否单例
    public bool HasMask;     // 是否有遮罩
}

public enum UIName
{
    StartDecide,
    Move,
    Money,
    City,
    Card,
    Lottery,
    Talk,
    Dest,
    // 可以继续添加其他UI名称
    MessageBox, // 通用消息弹窗
}

public static class UIConfig
{
    public static readonly Dictionary<UIName, UIConfigData> Configs
        = new()
    {
        { UIName.StartDecide, new UIConfigData { PrefabUrl = "Battle/StartDecideUI", Layer = UIType.Main } },
        { UIName.Move, new UIConfigData { PrefabUrl = "Battle/MoveUI", Layer = UIType.Main } },
        { UIName.City, new UIConfigData { PrefabUrl = "Battle/CityGridUI", Layer = UIType.Main } },
        { UIName.Money, new UIConfigData { PrefabUrl = "Battle/MoneyGridUI", Layer = UIType.Main } },
        { UIName.Card, new UIConfigData { PrefabUrl = "Battle/CardGridUI", Layer = UIType.Main } },
        { UIName.Lottery, new UIConfigData { PrefabUrl = "Battle/LotteryGridUI", Layer = UIType.Main } },
        { UIName.Talk, new UIConfigData { PrefabUrl = "Talk/TalkUI", HasMask = true, Layer = UIType.Pop } },
        { UIName.Dest, new UIConfigData { PrefabUrl = "Dest/DestUI", Layer = UIType.Pop } }
    };

    public static UIConfigData GetConfig(UIName uiName)
    {
        if (Configs.TryGetValue(uiName, out var config))
        {
            return config;
        }
        else
        {
            UnityEngine.Debug.LogError($"UI配置未找到: {uiName}");
            return null;
        }
    }
}