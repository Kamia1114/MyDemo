using Battle.Manager;
using UnityEngine;

namespace Core.Plugin
{
    /// <summary>
    /// 固定摄像机视角和Y轴，右键长按可反向拖动摄像机的X、Z轴
    /// </summary>
    public class FixedAngleDragCamera : MonoBehaviour
    {
        public float dragSpeed = 10f;         // 拖动速度
        public float fixedY = 10f;            // 固定的Y轴高度
        public Vector3 fixedEulerAngles = new Vector3(45, 0, 0); // 固定的视角（欧拉角）
        public Vector2 startPosition = new Vector2(69f, 58f); // 默认位置
        private Vector3 lastMousePosition;
        private bool isDragging = false;
        private Bounds planeBounds;

        void Start()
        {
            // 初始化摄像机角度和Y轴
            transform.eulerAngles = fixedEulerAngles;
            transform.position = new Vector3(startPosition.x * GameConfig.GridInterval, fixedY, startPosition.y * GameConfig.GridInterval - 7);

            // 获取Plane的包围盒
            GameObject planeObj = GameObject.Find("Plane");
            if (planeObj != null)
            {
                Renderer renderer = planeObj.GetComponent<Renderer>();
                if (renderer != null)
                    planeBounds = renderer.bounds;
                else
                    planeBounds = new Bounds(planeObj.transform.position, new Vector3(1000, 0, 1000)); // 兜底
            }
            else
            {
                planeBounds = new Bounds(Vector3.zero, new Vector3(1000, 0, 1000)); // 兜底
            }
        }

        void Update()
        {
            // 固定摄像机角度和Y轴
            if (transform.eulerAngles != fixedEulerAngles)
                transform.eulerAngles = fixedEulerAngles;
            if (transform.position.y != fixedY)
            {
                Vector3 pos = transform.position;
                pos.y = fixedY;
                transform.position = pos;
            }

            // 右键按下开始拖动
            if (Input.GetMouseButtonDown(1))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            // 右键松开停止拖动
            if (Input.GetMouseButtonUp(1))
            {
                isDragging = false;
            }
            // 拖动中
            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                // 忽略极小的鼠标漂移，避免画面闪动
                // if (delta.sqrMagnitude < 500f && delta.sqrMagnitude > 0.1f)
                // {
                    // 反向移动摄像机的X、Z轴
                    Vector3 move = new Vector3(-delta.x, 0, -delta.y) * dragSpeed * Time.deltaTime;
                    Vector3 nextPos = transform.position + move;
                    nextPos.y = fixedY;

                    // 限制摄像机X、Z在Plane范围内，若超出则不移动
                    Vector3 min = planeBounds.min;
                    Vector3 max = planeBounds.max;
                    float clampedX = Mathf.Clamp(nextPos.x, min.x, max.x);
                    float clampedZ = Mathf.Clamp(nextPos.z, min.z, max.z);

                    // 只有目标位置在范围内才允许移动，防止一次大跳跃直接闪到边缘
                    if (Mathf.Approximately(nextPos.x, clampedX) && Mathf.Approximately(nextPos.z, clampedZ))
                    {
                        transform.position = nextPos;
                    }
                    // 否则只允许移动到边界
                    else
                    {
                        transform.position = new Vector3(clampedX, fixedY, clampedZ);
                    }
                // }
                lastMousePosition = Input.mousePosition;
            }
        }
    }
}