using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Utils
{
    public static class GameUtils
    {
        public static void SetTimeout(float seconds, Action callback)
        {
            // 优化：只创建一个全局TimeoutMono对象，避免每次new带来的开销
            TimeoutMono.Instance.StartCoroutine(DelayCoroutine(seconds, callback));
        }

        public static void BindButton<T>(object buttonObj, Action<T> onClick, T value = default, bool clearOld = true)
        {
            Button btn = null;
            if (buttonObj is Button b)
            {
                btn = b;
            }
            else if (buttonObj is GameObject go)
            {
                go.TryGetComponent(out btn);
            }
            if (btn != null)
            {
                if (clearOld)
                {
                    btn.onClick.RemoveAllListeners();
                }
                btn.onClick.AddListener(() => onClick?.Invoke(value));
            }
        }

        private static System.Collections.IEnumerator DelayCoroutine(float seconds, System.Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }
    }

    // 新增内部MonoBehaviour单例类
    public class TimeoutMono : MonoBehaviour
    {
        private static TimeoutMono _instance;
        public static TimeoutMono Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("TimeoutMono");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<TimeoutMono>();
                }
                return _instance;
            }
        }
    }
}

