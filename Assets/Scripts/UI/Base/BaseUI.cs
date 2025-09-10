using Core.Mgr;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour, IBaseUI
{
    protected UIName UiName { get; set; }
    protected GameUIController uiCtrl;
    public void SetViewModel(GameUIController gameUIController)
    {
        uiCtrl = gameUIController;
    }
    public virtual void SetUIName(UIName name)
    {
        UiName = name;
    }
    public UIName UIName => UiName;
    public virtual void Show()
    {
        uiCtrl.OnUpdateUI += UpdateUI;
        uiCtrl.OnShowMessage += OnShowMessage;
        UpdateUI();
    }
    public virtual void Init(object[] args) { }
    public virtual void Hide()
    {
        uiCtrl.OnUpdateUI -= UpdateUI;
        uiCtrl.OnShowMessage -= OnShowMessage;
    }
    public void Close()
    {
        UIManager.Instance.ClosePanel(UiName);
    }
    public virtual void ResetUI() { }

    protected virtual void UpdateUI() { }

    protected virtual void OnShowMessage(string message) { }
}