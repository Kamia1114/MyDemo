using UnityEngine;

namespace Core.Plugin
{
    public class FollowUI : MonoBehaviour
    {
        public Transform targetCube; // 要跟随的Cube
        public Vector3 offset = new Vector3(0, 1.5f, -1.0f); // 相对于Cube的偏移量

        void Start()
        {
            if (targetCube != null)
            {
                // 只跟随位置，不继承缩放和旋转
                transform.position = targetCube.position + offset;
                transform.rotation = Quaternion.Euler(30, 0, 0);
                // 让Canvas始终面向摄像机
                // transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            }
        }

        // void Update()
        // {
        //     if (targetCube != null)
        //     {
        //         // 只跟随位置，不继承缩放和旋转
        //         transform.position = targetCube.position + offset;

        //         // 让Canvas始终面向摄像机
        //         // transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        //     }
        // }
    }
}
