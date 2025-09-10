using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class UIRoundedImage : MonoBehaviour
{
    [Range(0, 1)]
    public float radius = 0.2f; // 圆角半径（0~1，1为最大）
    private float lastRadius = -1f;
    private Material roundedMat;
    private Image image;

    void Awake()
    {
        SetupMaterial();
    }

    void OnEnable()
    {
        SetupMaterial();
    }

    void OnValidate()
    {
        SetupMaterial();
        UpdateRadius();
    }

    private void SetupMaterial()
    {
        image = GetComponent<Image>();
        if (image == null) return;
        Shader shader = Shader.Find("UI/RoundedCorners");
        if (shader == null)
        {
            Debug.LogWarning("未找到UI/RoundedCorners Shader，圆角效果可能无效");
            return;
        }
        if (roundedMat == null || image.material == null || image.material.shader != shader)
        {
            roundedMat = new Material(shader);
            image.material = roundedMat;
        }
        UpdateRadius();
    }

    private void UpdateRadius()
    {
        if (roundedMat != null)
        {
            roundedMat.SetFloat("_Radius", radius);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        // 编辑器下，参数变动时实时刷新圆角效果
        if (!Application.isPlaying && Mathf.Abs(lastRadius - radius) > 0.0001f)
        {
            UpdateRadius();
            lastRadius = radius;
        }
#endif
    }
}
