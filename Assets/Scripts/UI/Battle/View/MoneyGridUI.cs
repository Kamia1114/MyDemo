using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MoneyType
{
    ///<summary>
    /// 收入
    /// </summary>
    Income, 
    ///<summary>
    /// 支出
    /// </summary>
    Expense
}

public class MoneyGridUI : TalkBaseUI
{
    [SerializeField]
    private Transform uiTop;
    [SerializeField]
    private TextMeshProUGUI randomMoneyText;

    private TextMeshProUGUI stationNameText;
    private TextMeshProUGUI targetStationNameText;
    private TextMeshProUGUI targetDistanceText;
    private TextMeshProUGUI curYearText;
    private TextMeshProUGUI curMonthText;
    private TextMeshProUGUI curMoneyText;
    [SerializeField]
    private Image moneyBgImage;

    private MoneyType moneyType;
    private int curRandomMoney;
    private bool isClick = false;
    private float lastRandomTime = 0f;

    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    protected void Start()
    {
        OnShowMessage($"{uiCtrl.GetCurrentPlayerName()}\n到达了({(moneyType == MoneyType.Income ? "加钱站" : "减钱站")})");
    }

    public override void Init(object[] args = null)
    {
        if (args != null && args.Length > 0 && args[0] is MoneyType type)
        {
            moneyType = type;
            switch (moneyType)
            {
                case MoneyType.Income:
                    moneyBgImage.color = Color.blue;

                    break;
                case MoneyType.Expense:
                    moneyBgImage.color = Color.red;
                    break;
            }
        }
        else
        {
            Debug.LogError("MoneyGridUI Init参数错误，缺少MoneyType");
        }
    }

    void Update()
    {
        if (!isClick && Time.time - lastRandomTime > 0.4f)
        {
            curRandomMoney = (int)Math.Round((double)(UnityEngine.Random.Range(1, 6) * 1000));
            randomMoneyText.text = curRandomMoney + "万";
            lastRandomTime = Time.time;
        }
        if (Input.GetMouseButtonDown(0) && !isClick)
        {
            OnLeftMouseClick();
        }
    }

    private void InitUI()
    {
        // 初始化UI元素
        stationNameText = uiTop.Find("txt_stationName").GetComponent<TextMeshProUGUI>();
        targetStationNameText = uiTop.Find("txt_targetStationName").GetComponent<TextMeshProUGUI>();
        targetDistanceText = uiTop.Find("txt_targetDistance").GetComponent<TextMeshProUGUI>();
        curYearText = uiTop.Find("txt_curYear").GetComponent<TextMeshProUGUI>();
        curMonthText = uiTop.Find("txt_curMonth").GetComponent<TextMeshProUGUI>();
        curMoneyText = uiTop.Find("txt_curMoney").GetComponent<TextMeshProUGUI>();

    }

    protected override void UpdateUI()
    {
        PlayerModel currentPlayer = uiCtrl.GetCurrentPlayerModel();
        stationNameText.text = uiCtrl.GetCurStationName();
        targetStationNameText.text = uiCtrl.GetDestStationName();
        targetDistanceText.text = uiCtrl.GetTargetSteps().ToString();
        curYearText.text = uiCtrl.Year;
        curMonthText.text = uiCtrl.Month;
        curMoneyText.text = currentPlayer.PlayerAssets.Money + "万";
    }

    private void OnLeftMouseClick()
    {
        isClick = true;
        uiCtrl.GetCurrentPlayerModel().ChangeMoney(moneyType == MoneyType.Income ? curRandomMoney : -curRandomMoney);
        List<string> messages = new()
        {
            $"{uiCtrl.GetCurrentPlayerName()}\n{(moneyType == MoneyType.Income ? "获得" : "失去")}{curRandomMoney}万！",
            $"{uiCtrl.GetCurrentPlayerName()}当前资金{uiCtrl.GetCurrentPlayerModel().PlayerAssets.Money}万！"
        };
        string messageStr = string.Join("|", messages);
        OnShowMessage(messageStr);
    }

    protected override void OnTalkComplete()
    {
        if (isClick == false)
            return;
        /// <summary>
        /// 处理对话完成的逻辑
        /// </summary>
        Close();
        uiCtrl.PlayTurn();
    }
}