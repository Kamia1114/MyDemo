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

public class StartDecideUI : TalkBaseUI
{
    [SerializeField]
    private Transform uiTop;

    // [SerializeField]
    // private Transform uiBottom;

    [SerializeField]
    private Transform uiDecide;

    [SerializeField]
    private Transform uiDice;
    [SerializeField]
    private Transform uiCard;

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

    protected override void Awake()
    {
        base.Awake();
        InitUI();
    }

    void Start()
    {
        InitEvent();
    }

    void Update()
    {
        if (curUIState == StartDecideUIState.RollDice && isRolledDice == false)
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
        ChangeUIState(StartDecideUIState.Main);
    }

    private void InitEvent()
    {
        // 初始化事件
        GameUtils.BindButton(diceButton, ChangeUIState, StartDecideUIState.RollDice);
        GameUtils.BindButton(cardButton, ChangeUIState, StartDecideUIState.Card);
        // viewModel.OnUpdateUI += UpdateUI;
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
        //卡牌
        var cards = currentPlayer.PlayerAssets.Cards;
        GameObject content = uiCard.GetComponent<ScrollRect>().content.gameObject;
        // content.transform.DetachChildren();
        for (int i = content.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        if (cards.Count > 0)
        {
            GameObject cardPrefab = uiCard.transform.Find("item_card").gameObject;
            cardPrefab.SetActive(false);
            foreach (var card in cards)
            {
                Debug.Log($"卡牌ID: {card.cardId}, 数量: {card.times}");
                var cardCfg = ConfigManager.GetConfig<CardCfgTable>(card.cardId);
                if (cardCfg != null)
                {
                    GameObject item = Instantiate(cardPrefab, content.transform, false);
                    item.name = $"Card_{card.id}";
                    item.SetActive(true);
                    TextMeshProUGUI cardText = item.transform.Find("txt_name").GetComponent<TextMeshProUGUI>();
                    cardText.text = cardCfg.name;
                }
            }
        }
    }

    public void UseCard(Button btn)
    {
        // 打开卡牌界面
        int id = int.Parse(btn.name.Split('_')[1]);
        uiCtrl.UseCard(id);
    }

    private void ChangeUIState(StartDecideUIState newState)
    {
        curUIState = newState;
        switch (newState)
        {
            case StartDecideUIState.Main:
                uiTop.gameObject.SetActive(true);
                uiDecide.gameObject.SetActive(true);
                uiDice.gameObject.SetActive(false);
                uiCard.gameObject.SetActive(false);
                talkUI.gameObject.SetActive(false);
                break;
            case StartDecideUIState.RollDice:
                uiTop.gameObject.SetActive(false);
                uiDecide.gameObject.SetActive(false);
                uiDice.gameObject.SetActive(true);
                isRolledDice = false;
                playRandomDiceTime = Time.time;
                break;
            case StartDecideUIState.Card:
                // 打开卡牌界面
                uiTop.gameObject.SetActive(false);
                uiCard.gameObject.SetActive(true);
                talkUI.gameObject.SetActive(true);
                break;
            case StartDecideUIState.Option:
                // 打开选项界面
                break;
            default:
                break;
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
