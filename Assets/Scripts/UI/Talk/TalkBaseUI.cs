using UnityEngine;

public abstract class TalkBaseUI : BaseUI
{
    protected TalkUI talkUI;
    // 其他通用对话UI逻辑
    protected virtual void Awake()
    {
        // 初始化对话UI
        talkUI = GetComponentInChildren<TalkUI>();
        if (talkUI != null)
        {
            talkUI.OnTalkUIClose += OnTalkComplete;
        }
    }

    protected override void OnShowMessage(string message)
    {
        talkUI.SetMessage(message);
        talkUI.Play();
    }

    public void Play()
    {
            
    }

    protected virtual void OnTalkComplete()
    {
        // 处理对话UI关闭事件
    }
}