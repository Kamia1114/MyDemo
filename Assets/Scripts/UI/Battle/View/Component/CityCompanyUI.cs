using System;
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

    protected void Awake()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);
        bgImg.color = Color.white;
        // bgImg.color.WithAlpha(0);
    }

    public void SetData(CompanyCfgTable companyTable, GameUIController uiCtrl)
    {
        if (companyTable == null) return;
        SetBeSellState(uiCtrl.IsBeSellCompany(companyTable.ID));
        this.companyTable = companyTable;
        if (companyTable.money > uiCtrl.GetCurrentPlayerModel().PlayerProperty.Money)
        {
            bgImg.color = Color.red; // 标红背景表示买不起;
        }
        else
        {
            bgImg.color = Color.white;
        }
        nameText.text = companyTable.name;
        moneyText.text = companyTable.money + "万";
        ratioText.text = companyTable.ratio + "%";
    }

    private void SetBeSellState(bool isBeSell)
    {
        if (isBeSell)
        {
            nameText.color = Color.red;
            moneyText.color = Color.red;
            ratioText.color = Color.red;
        }
        else
        {
            nameText.color = Color.black;
            moneyText.color = Color.black;
            ratioText.color = Color.black;
        }
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
        // bgImg.color.WithAlpha(0);
        bgImg.color = Color.white;
    }

    private void OnDestroy()
    {
        buyButton.onClick.RemoveListener(OnBuyButtonClicked);
    }
}