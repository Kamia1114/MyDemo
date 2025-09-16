using Core.Enum;
using UnityEngine;

namespace Battle.Grid
{
    /// <summary>
    /// 地块视图，负责地块的显示和交互
    /// </summary>
    public class GridItem : MonoBehaviour
    {
        private int gridId;
        private GridViewModel viewModel;
        private GridModel gridModel;

        void Start()
        {
            Init();
        }

        public void SetData(int gId, GridViewModel vm)
        {
            gridId = gId;
            viewModel = vm;
            gridModel = viewModel.GetModel(gridId);
        }

        protected virtual void Init()
        {
            UpdateUI();
            InitEvent();
        }

        protected virtual void InitEvent()
        {
            gridModel.OnDataChanged += () => UpdateUI();
        }

        protected virtual void UpdateUI()
        {
            // 更新UI元素
            transform.position = gridModel.Coordinates;
            this.SetType(gridModel.GridType, gridModel.Arg);
        }

        public void SetType(GridTypeEnum gridType, string arg)
        {
            // 根据地块类型设置不同的材质或颜色
            if (TryGetComponent<Renderer>(out var renderer))
            {
                switch (gridType)
                {
                    case GridTypeEnum.City:
                        renderer.material.color = new Color(1f, 0.75f, 0.8f);
                        break;
                    case GridTypeEnum.Money:
                        if (arg == "+")
                        {
                            renderer.material.color = Color.green;
                        }
                        else
                        {
                            renderer.material.color = Color.red;
                        }
                        break;
                    case GridTypeEnum.Card:
                        renderer.material.color = Color.blue;
                        break;
                    case GridTypeEnum.Lottery:
                        renderer.material.color = Color.green; // 深绿色
                        break;
                    case GridTypeEnum.Fly:
                        renderer.material.color = Color.yellow; // 深绿色
                        break;
                    case GridTypeEnum.Shop:
                        renderer.material.color = Color.blue; // 深绿色
                        break;
                    // case GridTypeEnum.机场:
                    //     renderer.material.color = Color.white; // 深绿色
                    //     break;
                    default:
                        renderer.material.color = Color.white;
                        break;
                }
            }
        }

        // 监听点击事件
        private void OnMouseDown()
        {
            // 这里可以调用ViewModel或抛出事件
            viewModel?.OnGridClicked(gridId);
        }
        
        // 进入触发器时调用
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("进入触发器：" + other.gameObject.name);
        }

        // 在触发器内每帧调用
        private void OnTriggerStay(Collider other)
        {
            Debug.Log("在触发器内：" + other.gameObject.name);
        }

        // 离开触发器时调用
        private void OnTriggerExit(Collider other)
        {
            Debug.Log("离开触发器：" + other.gameObject.name);
        }
    }
}
