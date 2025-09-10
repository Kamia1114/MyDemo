using UnityEngine;
using Battle.Player;
using Battle.Manager;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    private PlayerViewModel playerViewModel;

    private Transform cameraTransform;
    private PlayerItem currentPlayerItem;
    private bool isFollowing = false;

    private void Awake()
    {
        cameraTransform = this.transform;
    }

    private void Start()
    {
        
    }

    public void SetPlayerViewModel(PlayerViewModel viewModel)
    {
        playerViewModel = viewModel;

        if (playerViewModel != null)
        {
            // 订阅事件
            PlayerManager.Instance.OnCurPlayerChanged += OnCurPlayerChanged;
            playerViewModel.OnPlayerActionChanged += OnPlayerActionChanged;
            // 初始化摄像机位置
            // OnCurPlayerChanged();
        }
    }

    private void OnCurPlayerChanged()
    {
        var player = PlayerManager.Instance.CurrentPlayer;
        if (player != null)
        {
            currentPlayerItem = FindPlayerItem(player.Index);
            // 获取当前PlayerItem（假设有方法或映射获取）
            Vector3 pos = player.GetPosition();
            this.StopAllCoroutines();
            StartCoroutine(SmoothMove(new Vector3(pos.x, cameraTransform.position.y, pos.z - 6)));
        }
    }

    private IEnumerator SmoothMove(Vector3 targetPosition)
    {
        float duration = 0.5f; // 平滑移动持续时间
        float elapsed = 0f;
        Vector3 startingPos = cameraTransform.position;

        while (elapsed < duration)
        {
            cameraTransform.position = Vector3.Lerp(startingPos, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraTransform.position = targetPosition;
    }

    private void OnPlayerActionChanged(PlayerAction action)
    {
        // 当玩家进入移动状态时，开始跟随
        isFollowing = action == PlayerAction.Moving;
        if (isFollowing && currentPlayerItem == null)
        {
            currentPlayerItem = FindPlayerItem(PlayerManager.Instance.CurrentPlayer.Index);
        }
    }

    private void LateUpdate()
    {
        if (isFollowing && currentPlayerItem != null)
        {
            Vector3 pos = currentPlayerItem.transform.position;
            cameraTransform.position = new Vector3(pos.x, cameraTransform.position.y, pos.z - 6);
        }
    }

    // 你需要根据你的项目结构实现此方法，获取当前player对应的PlayerItem
    private PlayerItem FindPlayerItem(int playerIndex)
    {
        // 示例：假设所有PlayerItem都挂在"Cars"下
        var carPanel = GameObject.Find("Cars");
        if (carPanel == null) return null;
        foreach (Transform child in carPanel.transform)
        {
            var item = child.GetComponent<PlayerItem>();
            if (item != null && item.PlayerIndex == playerIndex)
                return item;
        }
        return null;
    }
}
