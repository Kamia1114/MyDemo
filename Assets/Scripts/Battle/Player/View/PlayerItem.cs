using System.Collections;
using Battle.Player;
using Core.Mgr;
using Core.Table;
using UnityEngine;

/// <summary>
/// 只负责UI和表现（如角色动画、位置、UI刷新）。
/// 监听PlayerViewModel的事件，更新UI和表现。
/// 不处理业务逻辑和数据存储。
/// </summary>
public class PlayerItem : MonoBehaviour
{

    private int playerIndex;
    public int PlayerIndex => playerIndex;
    private PlayerViewModel viewModel;

    // Start is called before the first frame update
    void Start()
    {
        // 初始化视图
        Init();
    }

    public void SetData(int index, PlayerViewModel vm)
    {
        playerIndex = index;
        viewModel = vm;
    }

    protected virtual void Init()
    {
        InitEvent();
        InitData();
    }

    protected virtual void InitEvent()
    {
    }

    protected virtual void InitData()
    {
        transform.position = viewModel.GetPlayerPosition(playerIndex);
    }

    // 轮到当前玩家回合，更新视图
    public void OnCurPlayerTurnChanged(bool isTurn)
    {
        if (isTurn && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    // 玩家位置更新，更新视图位置
    public void OnPlayerPositionChanged(Vector3 newPosition)
    {
        // 这里可以添加平滑移动的逻辑
        if (this.isActiveAndEnabled)
        {
            this.StopAllCoroutines();
            StartCoroutine(SmoothMove(newPosition));
        }
        else
        {
            transform.position = newPosition;

        }
    }

    private IEnumerator SmoothMove(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;
        // 计算方向并设置Y轴朝向
        Vector3 dir = targetPosition - startingPos;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up);
            transform.rotation = lookRot;
        }
        while (elapsedTime < GameConfig.PlayerSpeed)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / GameConfig.PlayerSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        viewModel.NotifyPlayerArrived();
    }
}
