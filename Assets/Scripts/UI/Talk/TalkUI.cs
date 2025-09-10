using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Mgr;
using TMPro;
using UnityEngine;

public class TalkUI : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds0_05 = new(0.02f);
    [SerializeField]
    private TextMeshProUGUI talkText;
    [SerializeField]
    private bool isAutoHide;
    [SerializeField]
    private bool isPlayWait = true;
    private List<string> talkInfo;
    private bool isPlaying = false;
    private string curPlayMessage = "";
    public event Action OnTalkUIClose;

    public void SetMessage(string args)
    {
        talkInfo = args.Split('|').ToList();
    }

    public void Play()
    {
        // 显示对话框
        gameObject.SetActive(true);
        if (isPlayWait && isPlaying)
        {
            // 如果正在播放且需要等待播完，立即显示完整文本
            StopAllCoroutines();
            talkText.text = curPlayMessage;
            isPlaying = false;
            return;
        }
        PlayNextTalk();
    }

    private void Update()
    {
        // 更新对话框状态
        if (gameObject.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isPlayWait && isPlaying)
            {
                // 如果正在播放且需要等待播完，立即显示完整文本
                StopAllCoroutines();
                talkText.text = curPlayMessage;
                isPlaying = false;
            }
            else if (!isPlaying)
            {
                PlayNextTalk();
            }
        }
    }

    public void PlayNextTalk()
    {
        if (talkInfo != null && talkInfo.Count > 0)
        {
            string first = talkInfo[0];
            talkInfo.RemoveAt(0);
            PlayTalkText(first); // 取出并删除第一条信息
        }
        else
        {
            OnTalkUIClose?.Invoke();
            if (isAutoHide)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void PlayTalkText(string text)
    {
        // 播放对话文本的动画或效果
        isPlaying = true;
        curPlayMessage = text;
        StartCoroutine(ShowTextCoroutine());
    }

    private IEnumerator ShowTextCoroutine()
    {
        talkText.text = "";
        foreach (char c in curPlayMessage)
        {
            talkText.text += c;
            yield return _waitForSeconds0_05; // 每个字符显示的间隔时间
        }
        isPlaying = false;
        // 3秒内未点击则自动播放下一条
        float timer = 0f;
        while (timer < 3f)
        {
            if (Input.GetMouseButtonDown(0))
                yield break;
            timer += Time.deltaTime;
            yield return null;
        }
        PlayNextTalk();
    }
}