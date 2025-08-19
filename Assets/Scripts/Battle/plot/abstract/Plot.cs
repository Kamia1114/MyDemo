using UnityEngine;

/// <summary>
/// 地块抽象基类，所有类型的地块都继承自此
/// </summary>
public abstract class Plot : MonoBehaviour
{
    [Header("基础属性")]
    [Tooltip("地块唯一标识")]
    [SerializeField] protected int plotId;

    [Tooltip("当前地块拥有者")]
    [SerializeField] protected int ownerId;

    [Tooltip("地块类型")]
    [SerializeField] public PlotType PlotType { get; protected set; }
    
    [Tooltip("地块在游戏板上的坐标")]
    [SerializeField] protected Vector2Int coordinates;
    
    [Tooltip("与之相连的其他地块")]
    [SerializeField] protected Plot[] connectedPlots;


    // 属性访问器
    public int PlotId => plotId;
    public int OwnerId => ownerId;
    public Vector2Int Coordinates => coordinates;
    public Plot[] ConnectedPlots => connectedPlots;

    /// <summary>
    /// 当玩家进入该地块时触发的行为
    /// </summary>
    /// <param name="player">进入地块的玩家</param>
    public abstract void OnPlayerEnter(BasePlayer player);

    /// <summary>
    /// 初始化地块连接关系
    /// </summary>
    /// <param name="connections">要连接的地块数组</param>
    public virtual void InitializeConnections(Plot[] connections)
    {
        connectedPlots = connections;
    }
}
