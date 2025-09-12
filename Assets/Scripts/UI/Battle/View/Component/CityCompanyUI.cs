using System;
using Core.Mgr;
using Core.Table;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CityCompanyUI : MonoBehaviour
{
    public Image bgImg;
    public Image typeImg;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ratioText;
    public Button buyButton;
    public event Action<CompanyCfgTable> OnMouseOverAction;
    public event Action<CompanyCfgTable> OnMouseClickAction;
    private CompanyCfgTable companyTable;
    private GameUIController uiCtrl;

    protected void Awake()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        // bgImg.color = Color.white;
        // bgImg.color.WithAlpha(0);
    }

    public void SetData(CompanyCfgTable companyTable, GameUIController uiCtrl = null)
    {
        if (companyTable == null) return;
        if (uiCtrl != null)
        {
            this.uiCtrl = uiCtrl;
        }
        this.companyTable = companyTable;
        SetBeSellState(companyTable);

    }

    private void SetBeSellState(CompanyCfgTable companyTable)
    {
        if (companyTable.money > uiCtrl.GetCurrentPlayerModel().PlayerAssets.Money)
        {
            // 标红背景表示买不起;
            bgImg.color = new Color32(190, 100, 90, 255); 
            nameText.color = Color.red;
            moneyText.color = Color.red;
            ratioText.color = Color.red;
        }
        else if (uiCtrl.IsBeBuyCompany(companyTable.ID))
        {
            // 已经被卖掉了;
            bgImg.color = Color.gray; // 标灰背景表示已被购买
        }
        else
        {
            // 可购买状态
            bgImg.color = Color.white;
            nameText.color = Color.black;
            moneyText.color = Color.black;
            ratioText.color = Color.black;
        }
        nameText.text = companyTable.name;
        moneyText.text = companyTable.money + "万";
        ratioText.text = companyTable.ratio + "%";
    }

    private void OnBuyButtonClicked()
    {
        Debug.Log("Company Buy Button Clicked");
        OnMouseClickAction?.Invoke(companyTable);
    }

    public void OnMouseOver()
    {
        Debug.Log("Mouse Over City Company");
        // bgImg.color.WithAlpha(1.0f);
        bgImg.color = Color.yellow;
        OnMouseOverAction?.Invoke(companyTable);
    }

    public void OnMouseExit()
    {
        Debug.Log("Mouse Exit City Company");
        if (companyTable.money > uiCtrl.GetCurrentPlayerModel().PlayerAssets.Money)
        {
            bgImg.color = new Color32(190, 100, 90, 255); // 标红背景表示买不起;
        }
        else if (uiCtrl.IsBeBuyCompany(companyTable.ID))
        {
            bgImg.color = Color.gray; // 标灰背景表示已被购买
        }
        else
        {
            bgImg.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        buyButton.onClick.RemoveListener(OnBuyButtonClicked);
    }
}