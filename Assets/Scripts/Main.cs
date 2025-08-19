using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneChangeButton : MonoBehaviour
{

    private Button _button;

    private void Awake()
    {
        // 获取按钮组件
        _button = GetComponent<Button>();
        
        // 确保按钮存在
        if (_button != null)
        {
            // 绑定点击事件
            _button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("场景切换按钮脚本所在物体上没有Button组件！");
        }
    }

    /// <summary>
    /// 按钮点击事件处理
    /// </summary>
    private void OnButtonClick()
    {
        try
        {
            // 切换场景
            SceneManager.LoadScene("ChinaMap");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"切换场景失败: {e.Message}");
        }
    }
}
