using UnityEngine;

namespace Core.Plugin
{

    [RequireComponent(typeof(Canvas))]
    public class FaceCamera : MonoBehaviour
    {
        private Camera mainCamera;
        private Canvas uiCanvas;

        void Start()
        {
            // 获取主摄像机
            mainCamera = Camera.main;

            // 获取当前物体上的Canvas组件
            uiCanvas = GetComponent<Canvas>();

            // 确保Canvas设置为World Space模式
            if (uiCanvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogWarning("Canvas should be in World Space mode for proper functionality");
                uiCanvas.renderMode = RenderMode.WorldSpace;
            }
        }

        void LateUpdate()
        {
            if (mainCamera != null)
            {
                // 计算面向摄像机的旋转
                Quaternion targetRotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);

                // 应用旋转，只保留Y轴旋转（可选，根据需求调整）
                transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

                // 或者使用下面的代码让面板完全面向摄像机（包括X和Z轴旋转）
                // transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                //                 mainCamera.transform.rotation * Vector3.up);
            }
        }
    }
}