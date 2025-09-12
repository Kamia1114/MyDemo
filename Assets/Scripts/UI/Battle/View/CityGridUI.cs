using Core.Table;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

enum CityGridState
{
    Decide,
    Company,
    CompanyBuy,
    Card,
    CardUse,
    Operate,
}

public class CityGridUI : TalkBaseUI
{

    [SerializeField]
    private GameObject decidePanel;
    [SerializeField]
    private ScrollRect companyScrollRect;
    [SerializeField] private TextMeshProUGUI cityNameText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerMoneyText;

    [SerializeField] private Button buyButton;
    [SerializeField] private Button autoBuyButton;
    [SerializeField] private Button cardButton;
    [SerializeField] private Button operateButton;
    [SerializeField] private Button cancelButton;

    // [SerializeField] private TalkUI talkUI;

    [SerializeField] private GameObject prefabCompanyUI;

    private CityGridState curState = CityGridState.Decide;

    void Start()
    {
        InitUI();
        InitEvent();
        InitData();
    }

    void Update()
    {
        // 更新对话框状态
        if (Input.GetMouseButtonDown(1))
        {
            switch (curState)
            {
                case CityGridState.Company:
                    ChangeState(CityGridState.Decide);
                    break;
                case CityGridState.CompanyBuy:
                    ChangeState(CityGridState.Company);
                    break;
                case CityGridState.Card:
                    ChangeState(CityGridState.Decide);
                    break;
                case CityGridState.CardUse:
                    ChangeState(CityGridState.Card);
                    break;
                case CityGridState.Operate:
                    ChangeState(CityGridState.Decide);
                    break;
                default:
                    break;
            }
        }
    }

    private void InitUI()
    {
        ChangeState(CityGridState.Decide);
    }

    private void InitEvent()
    {
        // talkUI = UIManager.Instance.ShowUI<TalkUI>("TalkUI");
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        // autoBuyButton.onClick.AddListener(OnAutoBuyButtonClicked);
        // cardButton.onClick.AddListener(OnCardButtonClicked);
        // operateButton.onClick.AddListener(OnOperateButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
    }

    private void InitData()
    {
        var companies = uiCtrl.GetCurStationCompanies(); // 初始化
        companyScrollRect.content.DetachChildren();
        foreach (var companyTable in companies)
        {
            var companyUIObj = Instantiate(prefabCompanyUI, companyScrollRect.content, false);
            companyUIObj.name = $"Company_{companyTable.ID}";
            var companyUI = companyUIObj.GetComponent<CityCompanyUI>();
            companyUI.OnMouseOverAction += OnCompanyMouseOver;
            companyUI.OnMouseClickAction += OnCompanyMouseClick;
            companyUI.SetData(companyTable, uiCtrl);
        }
        cityNameText.text = uiCtrl.GetCurStationName();
        playerNameText.text = uiCtrl.GetCurrentPlayerName();
        playerMoneyText.text = uiCtrl.GetCurrentPlayerProperty().Money+"万";
    }

    protected override void UpdateUI()
    {
        var companies = uiCtrl.GetCurStationCompanies(); // 初始化
        foreach (var companyTable in companies)
        {
            Transform childTransform = null;
            foreach (Transform child in companyScrollRect.content)
            {
                if (child.name == $"Company_{companyTable.ID}")
                {
                    childTransform = child;
                    break;
                }
            }
            if (childTransform != null)
            {
                var companyUI = childTransform.gameObject.GetComponent<CityCompanyUI>();
                companyUI.SetData(companyTable);
            }
        }
        playerMoneyText.text = uiCtrl.GetCurrentPlayerProperty().Money+"万";
    }

    private void ChangeState(CityGridState newState)
    {
        curState = newState;
        switch (newState)
        {
            case CityGridState.Decide:
                decidePanel.SetActive(true);
                companyScrollRect.gameObject.SetActive(false);
                // cardPanel.SetActive(false);
                // operatePanel.SetActive(false);
                break;
            case CityGridState.Company:
                decidePanel.SetActive(false);
                companyScrollRect.gameObject.SetActive(true);
                break;
            case CityGridState.Card:
                decidePanel.SetActive(false);
                // cardPanel.SetActive(true);
                break;
            case CityGridState.Operate:
                decidePanel.SetActive(false);
                // operatePanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void OnBuyButtonClicked()
    {
        Debug.Log("Buy Button Clicked");
        // viewModel.BuyCurrentCity();
        ChangeState(CityGridState.Company);
    }

    private void OnCancelButtonClicked()
    {
        Debug.Log("Cancel Button Clicked");
        uiCtrl.PlayTurn();
    }

    private void OnCompanyMouseOver(CompanyCfgTable companyTable)
    {
        // 显示提示信息
        OnShowMessage($"{companyTable.name}\n[公司类型尚未配置]\n预计收益{companyTable.money * companyTable.ratio / 100}万!");
        talkUI.SetPlayWait(false);
    }

    private void OnCompanyMouseClick(CompanyCfgTable companyTable)
    {
        // 购买公司
        uiCtrl.BuyCompany(companyTable);
    }

}