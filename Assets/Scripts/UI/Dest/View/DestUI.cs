using Battle.Manager;
using TMPro;
using UnityEngine;

public class DestUI : TalkBaseUI
{

    [SerializeField]
    private TextMeshProUGUI destNameText;

    // Start is called before the first frame update
    void Start()
    {
        destNameText.text = GridManager.Instance.StationDestGridID.ToString();
        OnShowMessage($"恭喜你到达了目的地|{destNameText.text}|请点击屏幕继续");
        Play();
    }

    protected override void OnTalkComplete()
    {
        /// <summary>
        /// 处理对话完成的逻辑
        /// </summary>
        BattleManager.Instance.ArrivedResult();
        Close();
    }
}
