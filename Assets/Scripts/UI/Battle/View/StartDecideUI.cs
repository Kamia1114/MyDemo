using System;
using Core.Mgr;
using Core.Table;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core.Utils;

enum StartDecideUIState
{
    Main,
    RollDice,
    Card,
    Option
}

public class StartDecideUI : BaseUI
{
    [SerializeField]
    private Transform uiTop;

    [SerializeField]
    private Transform uiBottom;

    [SerializeField]
    private Transform uiDecide;

    [SerializeField]
    private Transform uiDice;

    private TextMeshProUGUI stationNameText;
    private TextMeshProUGUI targetStationNameText;
    private TextMeshProUGUI targetDistanceText;
    private TextMeshProUGUI curYearText;
    private TextMeshProUGUI curMonthText;
    private TextMeshProUGUI curMoneyText;
    private TextMeshProUGUI curDiceText;

    private Button diceButton;
    private Button cardButton;
    private Button operationButton;

    private StartDecideUIState curUIState = StartDecideUIState.Main;

    private readonly float playRandomDiceInterval = 0.2f;
    private float playRandomDiceTime = 0.0f;

    private bool isRolledDice = false;

    void Awake()
    {
        InitUI();
    }

    void Start()
    {
        InitEvent();
    }

    void OnEnable()
    {
        isRolledDice = false;
    }

    void Update()
    {
        if (curDiceText != null && isRolledDice == false && curUIState == StartDecideUIState.RollDice)
        {
            if (Time.time - playRandomDiceTime > playRandomDiceInterval)
            {
                playRandomDiceTime = Time.time;
                curDiceText.text = UnityEngine.Random.Range(1, 7).ToString();
            }
        }
        if (curUIState != StartDecideUIState.Main)
        {
            // 检测鼠标左击（左键按下）
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseClick();
            }

            // 检测鼠标右击（右键按下）
            if (Input.GetMouseButtonDown(1))
            {
                OnRightMouseClick();
            }
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
        diceButton = uiDecide.Find("btn_dice").GetComponent<Button>();
        cardButton = uiDecide.Find("btn_card").GetComponent<Button>();
        operationButton = uiDecide.Find("btn_operation").GetComponent<Button>();
        curDiceText = uiDice.Find("txt_dice").GetComponent<TextMeshProUGUI>();
        uiDice.gameObject.SetActive(false);
    }

    private void InitEvent()
    {
        // 初始化事件
        GameUtils.BindButton(diceButton, ChangeUIState, StartDecideUIState.RollDice);
        // viewModel.OnUpdateUI += UpdateUI;
    }

    protected override void UpdateUI()
    {
        if (uiCtrl == null) return;
        PlayerModel currentPlayer = uiCtrl.GetCurrentPlayerModel();
        if (currentPlayer == null) return;
        stationNameText.text = uiCtrl.GetCurStationName();
        targetStationNameText.text = uiCtrl.GetDestStationName();
        targetDistanceText.text = uiCtrl.GetTargetSteps().ToString();
        curYearText.text = uiCtrl.Year;
        curMonthText.text = uiCtrl.Month;
        curMoneyText.text = currentPlayer.PlayerProperty.Money + "万";
    }

    private void ChangeUIState(StartDecideUIState newState)
    {
        curUIState = newState;
        if (newState == StartDecideUIState.Main)
        {
            uiTop.gameObject.SetActive(true);
            uiDecide.gameObject.SetActive(true);
            uiDice.gameObject.SetActive(false);
        }
        else if (newState == StartDecideUIState.RollDice)
        {
            uiTop.gameObject.SetActive(false);
            uiDecide.gameObject.SetActive(false);
            uiDice.gameObject.SetActive(true);
        }
    }

    private void OnLeftMouseClick()
    {
        // 处理左键点击事件
        switch (curUIState)
        {
            case StartDecideUIState.Main:
                // 主界面点击处理
                break;
            case StartDecideUIState.RollDice:
                // 掷骰子界面点击处理
                int diceResult = uiCtrl.RollDice();
                curDiceText.text = diceResult.ToString();
                isRolledDice = true;
                break;
            case StartDecideUIState.Card:
                // 卡牌界面点击处理
                OpenCard();
                break;
            case StartDecideUIState.Option:
                // 选项界面点击处理
                break;
        }
    }

    private void OnRightMouseClick()
    {
        // 处理右键点击事件
        switch (curUIState)
        {
            case StartDecideUIState.Main:
                // 主界面点击处理
                isRolledDice = false;
                break;
            case StartDecideUIState.RollDice:
                // 掷骰子界面点击处理，返回主界面
                ChangeUIState(StartDecideUIState.Main);
                break;
            case StartDecideUIState.Card:
                // 卡牌界面点击处理，返回主界面
                ChangeUIState(StartDecideUIState.Main);
                break;
            case StartDecideUIState.Option:
                // 选项界面点击处理，返回主界面
                ChangeUIState(StartDecideUIState.Main);
                break;
        }
    }

    private void OpenCard()
    {
        // 打开卡牌界面
    }

    public override void ResetUI()
    {
        ChangeUIState(StartDecideUIState.Main);
        isRolledDice = false;
    }

}
