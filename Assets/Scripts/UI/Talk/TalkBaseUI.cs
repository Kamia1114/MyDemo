using UnityEngine;

public abstract class TalkBaseUI : BaseUI
{
    protected TalkUI talkUI;
    // 其他通用对话UI逻辑
    void Awake()
    {
        // 初始化对话UI
        talkUI = GetComponentInChildren<TalkUI>();
        talkUI.OnTalkUIClose += OnTalkComplete;
    }

    protected override void OnShowMessage(string message)
    {
        talkUI.SetMessage(message);
    }

    public void Play()
    {
        talkUI.Play();
    }

    protected virtual void OnTalkComplete()
    {
        // 处理对话UI关闭事件
    }
}