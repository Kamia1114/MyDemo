using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Core.Mgr
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;
        public static UIManager Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
        }

        [SerializeField]
        private GameObject UIPanel;

        // 存储当前显示的弹窗
        private readonly Stack<GameObject> currentPanels = new();

        public void OpenUI(UIName panelName, object[] args = null)
        {
            if (!UIConfig.Configs.TryGetValue(panelName, out var config))
            {
                Debug.LogError($"UIConfig未配置面板: {panelName}");
                return;
            }
            GameObject panelPrefab = CreateManager.CreateUI(config.PrefabUrl);
            if (panelPrefab != null)
            {
                ShowPanel(panelPrefab, panelName, config, args);
            }
            else
            {
                Debug.LogError($"未能找到面板预制体: {config.PrefabUrl}");
            }
        }

        /// <summary>
        /// 显示UI面板
        /// </summary>
        /// <param name="panelPrefab">面板预制体</param>
        /// <param name="parent">父级容器，默认使用Canvas</param>
        private void ShowPanel(GameObject panelPrefab, UIName panelName, UIConfigData config, object[] args = null)
        {
            if (panelPrefab == null)
            {
                Debug.LogError("面板预制体不能为空！");
                return;
            }

            // 查找或创建Canvas作为默认父级
            // 优先查找名为PopUI的子物体
            Transform uiLayer = UIPanel.transform.Find(config.Layer.ToString());
            Transform panelParent = uiLayer != null ? uiLayer : CreateCanvas(config.Layer.ToString()).transform;
            if (config.IsSingleton)
            {
                foreach (var openPanel in currentPanels)
                {
                    if (openPanel.name == panelPrefab.name)
                    {
                        Debug.LogWarning($"单例面板已存在: {panelPrefab.name}");
                        return;
                    }
                }
            }
            if (config.Layer == UIType.Main || config.closeLayer)
            {
                CloseAllPanelsByLayer(config.Layer);
            }
            // 创建遮罩背景（用于点击空白区域关闭）
            GameObject mask = config.HasMask ? CreateMask(panelParent) : null;

            // 实例化面板
            GameObject panel = Instantiate(panelPrefab, panelParent);
            panel.name = panelPrefab.name; // 保持名字一致
            
            if (panel.TryGetComponent<BaseUI>(out var btBaseUI))
            {
                btBaseUI.SetUIName(panelName);
                btBaseUI.Init(args);
                btBaseUI.SetViewModel(transform.GetComponent<GameUIController>());
                btBaseUI.Show();
            } else if (panel.TryGetComponent<BaseUI>(out var baseUI))
            {
                baseUI.SetUIName(panelName);
                baseUI.Init(args);
                baseUI.Show();
            }
            
            // 确保面板在遮罩上方
            panel.transform.SetAsLastSibling();

            // 存储弹窗和对应的遮罩
            currentPanels.Push(panel);
            if (mask != null)
            {
                currentPanels.Push(mask);
            }
        }

        /// <summary>
        /// 创建遮罩背景
        /// </summary>
        private GameObject CreateMask(Transform parent)
        {
            GameObject mask;
            
            mask = new GameObject("UIMask");
            mask.transform.SetParent(parent);
            
            // 添加Image组件作为背景
            Image maskImage = mask.AddComponent<Image>();
            maskImage.color = new Color(0, 0, 0, 0.8f); // 半透黑
            
            // 设置 RectTransform 铺满屏幕
            RectTransform rect = mask.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // 添加按钮组件用于检测空白区域点击
            Button maskButton = mask.GetComponent<Button>() ?? mask.AddComponent<Button>();
            maskButton.onClick.AddListener(CloseTopPanel);
            
            return mask;
        }

        /// <summary>
        /// 关闭最上层的面板
        /// </summary>
        public void CloseTopPanel()
        {
            if (currentPanels.Count >= 2)
            {
                GameObject topPanel = currentPanels.Pop();
                if (topPanel.TryGetComponent<BaseUI>(out var baseUI))
                {
                    baseUI.Hide();
                }
                // 先销毁面板
                Destroy(topPanel);
                // 再销毁对应的遮罩
                Destroy(currentPanels.Pop());
            }
        }

        /// <summary>
        /// 关闭所有面板
        /// </summary>
        public void CloseAllPanels()
        {
            while (currentPanels.Count > 0)
            {
                GameObject topPanel = currentPanels.Pop();
                if (topPanel.TryGetComponent<BaseUI>(out var baseUI))
                {
                    baseUI.Hide();
                }
                Destroy(topPanel);
            }
        }
        
        public void CloseAllPanelsByLayer(UIType layer)
        {
            Stack<GameObject> tempStack = new Stack<GameObject>();
            while (currentPanels.Count > 0)
            {
                GameObject top = currentPanels.Pop();
                if (top.TryGetComponent<BaseUI>(out var baseUI))
                {
                    if (UIConfig.Configs.TryGetValue(baseUI.UIName, out var config) && config.Layer == layer)
                    {
                        baseUI.Hide();
                        Destroy(top);
                        // 如果有遮罩也一并销毁
                        if (currentPanels.Count > 0 && currentPanels.Peek().name == "UIMask")
                        {
                            Destroy(currentPanels.Pop());
                        }
                    }
                    else
                    {
                        tempStack.Push(top);
                    }
                }
                else
                {
                    tempStack.Push(top);
                }
            }
            // 还原栈顺序
            while (tempStack.Count > 0)
            {
                currentPanels.Push(tempStack.Pop());
            }
        }

        /// <summary>
        /// 通过uiName关闭对应的UI（如有遮罩也一并关闭）
        /// </summary>
        public void ClosePanel(UIName uiName)
        {
            if (!UIConfig.Configs.TryGetValue(uiName, out var config))
            {
                Debug.LogError($"UIConfig未配置面板: {uiName}");
                return;
            }
            // 查找栈中对应的panel和mask
            Stack<GameObject> tempStack = new Stack<GameObject>(); bool found = false;
            while (currentPanels.Count > 0)
            {
                GameObject top = currentPanels.Pop();
                if (!found && top.name == uiName.ToString())
                {
                    found = true;
                }
                if (found)
                {
                    if (top.TryGetComponent<BaseUI>(out var baseUI))
                    {
                        baseUI.Hide();
                    }
                    Destroy(top);
                    if (config.HasMask && currentPanels.Count > 0)
                    {
                        Destroy(currentPanels.Pop());
                    }
                    break;
                }
                else
                {
                    tempStack.Push(top);
                }
            }
            // 还原栈顺序
            while (tempStack.Count > 0)
            {
                currentPanels.Push(tempStack.Pop());
            }
        }

        /// <summary>
        /// 创建Canvas
        /// </summary>
        private GameObject CreateCanvas(string layerName)
        {
            // 创建新的Canvas
            GameObject canvasObj = new GameObject(layerName);
            canvasObj.name = layerName;
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            return canvasObj;
        }
    }
}