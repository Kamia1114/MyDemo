using UnityEngine;

namespace Core.Mgr
{
    public static class CreateManager
    {
        public static GameObject CreatePrefab(string prefabName)
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
            if (prefab == null)
            {
                Debug.LogError($"未找到预制体: Resources/Prefabs/{prefabName}");
                return null;
            }
            return prefab;
        }

        public static GameObject CreateLineRenderer()
        {
            GameObject lineObject = new GameObject();
            lineObject.name = "LineRenderer";
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.widthMultiplier = 0.3f;
            // 创建并设置绿色材质
            Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
            lineMaterial.color = Color.gray;
            lineRenderer.material = lineMaterial;
            // 线条始终面向上方
            lineRenderer.alignment = LineAlignment.TransformZ;
            lineRenderer.useWorldSpace = true;
            // lineRenderer.transform.up = Vector3.up;
            // 旋转X轴90度
            lineRenderer.transform.Rotate(Vector3.right, 90f);
            return lineObject;
        }

        public static GameObject CreateCarPrefab(string carPrefabName)
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Car/{carPrefabName}");
            if (prefab == null)
            {
                Debug.LogError($"未找到预制体: Resources/Prefabs/Car/{carPrefabName}");
                return null;
            }
            return prefab;
        }

        public static GameObject CreateUI(string uiPrefabName, string parentName = "")
        {
            GameObject prefab;
            string resPath = "";
            if (!string.IsNullOrEmpty(parentName))
            {
                resPath = $"UI/{parentName}/{uiPrefabName}";
            }
            else
            {
                if (uiPrefabName.Contains("/"))
                {
                    string[] paths = uiPrefabName.Split('/');
                    if (paths.Length > 0)
                    {
                        resPath = $"UI/{paths[0]}/{paths[1]}";
                    }
                }
                else
                {
                    resPath = $"UI/{uiPrefabName}/{uiPrefabName}UI";
                }
            }

            prefab = Resources.Load<GameObject>(resPath);
            if (prefab == null)
            {
                Debug.LogError($"未找到UI预制体: {resPath}");
                return null;
            }
            return prefab;
        }
    }
}
