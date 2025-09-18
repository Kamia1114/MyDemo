using System;
using Core.Mgr;
using Core.Table;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core.Utils;
using Core.Enum;
using System.Collections.Generic;

enum StartDecideUIState
{
    Main,
    RollDice,
    Card,
    Option,
    PlayerSelect,
    CardSelect,
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
    [SerializeField]
    private Transform item_dice;
    [SerializeField]
    private Transform uiPlayer;
    private TextMeshProUGUI stationNameText;
    private TextMeshProUGUI targetStationNameText;
    private TextMeshProUGUI targetDistanceText;
    private TextMeshProUGUI curYearText;
    private TextMeshProUGUI curMonthText;
    private TextMeshProUGUI curMoneyText;
    // private TextMeshProUGUI curDiceText;

    private Button diceButton;
    private Button cardButton;
    private Button operationButton;

    private StartDecideUIState curUIState = StartDecideUIState.Main;

    private readonly float playRandomDiceInterval = 0.2f;
    private float playRandomDiceTime = 0.0f;

    private bool isRolledDice = false;
    // 骰子数量
    private int diceCount = 1;
    // 当前使用的卡牌唯一ID
    private int curCardOnlyID = 0;

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
            if (diceCount > uiDice.childCount)
            {
                for (int i = uiDice.childCount; i < diceCount; i++)
                {
                    Instantiate(item_dice, uiDice);
                }
            }
            else if (diceCount < uiDice.childCount)
            {
                for (int i = uiDice.childCount - 1; i >= diceCount; i--)
                {
                    Destroy(uiDice.GetChild(i).gameObject);
                }
            }
            if (Time.time - playRandomDiceTime > playRandomDiceInterval)
            {
                playRandomDiceTime = Time.time;
                for (int i = 0; i < uiDice.childCount; i++)
                {
                    uiDice.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = UnityEngine.Random.Range(1, 7).ToString();
                }
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
                Debug.Log($"卡牌ID: {card.cardID}, 数量: {card.remainingTimes}");
                var cardCfg = ConfigManager.GetConfig<CardCfgTable>(card.cardID);
                if (cardCfg != null)
                {
                    GameObject item = Instantiate(cardPrefab, content.transform, false);
                    item.name = $"Card_{card.ID}_{card.cardID}";
                    item.SetActive(true);
                    TextMeshProUGUI cardText = item.transform.Find("txt_name").GetComponent<TextMeshProUGUI>();
                    cardText.text = cardCfg.name;
                }
            }
        }
    }

    public void UseCard(Button btn)
    {
        if (StartDecideUIState.PlayerSelect == curUIState)
        {
            Debug.Log("正在选择玩家，无法使用其他卡牌");
            return;
        }
        curCardOnlyID = int.Parse(btn.name.Split('_')[1]);
        int cardId = int.Parse(btn.name.Split('_')[2]);
        var cardCfg = ConfigManager.GetConfig<CardCfgTable>(cardId);
        // 根据卡牌类型处理不同逻辑
        switch (cardCfg.interactionType)
        {
            case InteractionTypeEnum.DiceCount: // 骰子数量卡
                OpenMoreDiceUI(cardCfg.param[0]);
                break;
            case InteractionTypeEnum.Player:
                //打开玩家选择界面
                OpenPlayerSelectUI(cardCfg.target);
                break;
            default:
                uiCtrl.UseCard(curCardOnlyID);
                break;
        }
    }

    public void SelectedPlayer(Button btn)
    {
        int playerIndex = int.Parse(btn.name.Split('_')[1]);
        Debug.Log($"选择玩家: {playerIndex}");
        uiCtrl.UseCard(curCardOnlyID, playerIndex);
        ChangeUIState(StartDecideUIState.Card);
    }

    private void ChangeUIState(StartDecideUIState newState)
    {
        curUIState = newState;
        uiTop.gameObject.SetActive(false);
        uiDecide.gameObject.SetActive(false);
        uiDice.gameObject.SetActive(false);
        uiCard.gameObject.SetActive(false);
        talkUI.gameObject.SetActive(false);
        uiPlayer.gameObject.SetActive(false);
        switch (newState)
        {
            case StartDecideUIState.Main:
                uiTop.gameObject.SetActive(true);
                uiDecide.gameObject.SetActive(true);
                break;
            case StartDecideUIState.RollDice:
                uiDice.gameObject.SetActive(true);
                isRolledDice = false;
                playRandomDiceTime = Time.time;
                break;
            case StartDecideUIState.Card:
                // 打开卡牌界面
                uiCard.gameObject.SetActive(true);
                talkUI.gameObject.SetActive(true);
                break;
            case StartDecideUIState.PlayerSelect:
                // 打开玩家选择界面
                uiCard.gameObject.SetActive(true);
                talkUI.gameObject.SetActive(true);
                uiPlayer.gameObject.SetActive(true);
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
                if (isRolledDice) return;
                int diceResult = uiCtrl.RollDice(diceCount);
                int remain = diceResult;
                for (int i = 0; i < diceCount; i++)
                {
                    int min = 1;
                    int max = Mathf.Min(6, remain - (diceCount - (i + 1)));
                    int value = (i == diceCount - 1) ? remain : UnityEngine.Random.Range(min, max + 1);
                    remain -= value;
                    uiDice.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
                }
                isRolledDice = true;
                if (curCardOnlyID != 0)
                {
                    // 使用了加骰子卡牌，返回卡牌界面
                    curCardOnlyID = 0;
                    diceCount = 1;
                    uiCtrl.UseCard(curCardOnlyID);
                }
                break;
        }
    }

    private void OnRightMouseClick()
    {
        // 处理右键点击事件
        switch (curUIState)
        {
            case StartDecideUIState.Main:
                ChangeUIState(StartDecideUIState.Main);
                // 主界面点击处理
                break;
            case StartDecideUIState.RollDice:
                // 掷骰子界面点击处理，返回主界面
                diceCount = 1;
                curCardOnlyID = 0;

                ChangeUIState(StartDecideUIState.Main);
                break;
            case StartDecideUIState.Card:
                // 卡牌界面点击处理，返回主界面
                ChangeUIState(StartDecideUIState.Main);
                break;
            case StartDecideUIState.PlayerSelect:
                // 玩家选择界面点击处理，返回卡牌界面
                ChangeUIState(StartDecideUIState.Card);
                break;
            case StartDecideUIState.CardSelect:
                // 玩家选择界面点击处理，返回卡牌界面
                ChangeUIState(StartDecideUIState.Card);
                break;
            case StartDecideUIState.Option:
                // 选项界面点击处理，返回主界面
                ChangeUIState(StartDecideUIState.Main);
                break;
        }
    }

    private void OpenMoreDiceUI(int count)
    {
        diceCount = count;
        ChangeUIState(StartDecideUIState.RollDice);
    }

    private void OpenPlayerSelectUI(TargetTypeEnum type)
    {
        List<PlayerModel> players = new();
        // 打开玩家选择界面
        switch (type)
        {
            case TargetTypeEnum.OtherOne:
                players = uiCtrl.GetOtherPlayerModels();
                break;
            case TargetTypeEnum.All:
                players = uiCtrl.GetAllPlayerModels();
                break;
            default:
                break;
        }
        GameObject playerPrefab = uiPlayer.transform.Find("item_player").gameObject;
        GameObject content = uiPlayer.GetComponent<ScrollRect>().content.gameObject;
        playerPrefab.SetActive(false);
        foreach (var player in players)
        {
            Debug.Log($"玩家ID: {player.Index}, 名称: {player.CharacterName}");
            GameObject item = Instantiate(playerPrefab, content.transform, false);
            item.name = $"Player_{player.Index}";
            item.SetActive(true);
            TextMeshProUGUI playerNameText = item.transform.Find("txt_name").GetComponent<TextMeshProUGUI>();
            playerNameText.text = player.CharacterName;
        }
        ChangeUIState(StartDecideUIState.PlayerSelect);
    }

    public override void ResetUI()
    {
        ChangeUIState(StartDecideUIState.Main);
        isRolledDice = false;
    }

}
