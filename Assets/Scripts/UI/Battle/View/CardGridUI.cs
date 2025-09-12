using System;
using System.Collections.Generic;
using Core.Mgr;
using Core.Table;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardGridUI : TalkBaseUI
{
    [SerializeField]
    private Transform uiTop;
    [SerializeField]
    private TextMeshProUGUI randomCardNameText;

    private TextMeshProUGUI stationNameText;
    private TextMeshProUGUI targetStationNameText;
    private TextMeshProUGUI targetDistanceText;
    private TextMeshProUGUI curYearText;
    private TextMeshProUGUI curMonthText;
    private TextMeshProUGUI curMoneyText;

    private CardCfgTable curCardCfg;
    private bool isClick = false;
    private float lastRandomTime = 0f;

    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    protected void Start()
    {
        OnShowMessage($"{uiCtrl.GetCurrentPlayerName()}\n到达了卡牌站)");
    }

    void Update()
    {
        if (!isClick && Time.time - lastRandomTime > 0.4f)
        {
            Dictionary<int, CardCfgTable> cardCfgsDic = ConfigManager.GetAllConfig<CardCfgTable>();
            var keys = new List<int>(cardCfgsDic.Keys);
            int randomIdx = UnityEngine.Random.Range(0, keys.Count);
            int randomKey = keys[randomIdx];
            curCardCfg = cardCfgsDic[randomKey];
            randomCardNameText.text = curCardCfg.name;
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
        OnShowMessage($"{uiCtrl.GetCurrentPlayerName()}\n获得{curCardCfg.name}！");
        uiCtrl.GetCurrentPlayerModel().AddCard(curCardCfg.ID);
        isClick = true;
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