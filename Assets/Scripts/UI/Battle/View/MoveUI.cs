using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveUI : BaseUI
{

    [SerializeField]
    private TextMeshProUGUI remainStepsText;
    [SerializeField]
    private TextMeshProUGUI targetStationNameText;
    [SerializeField]
    private TextMeshProUGUI targetStepsText;

    // Start is called before the first frame update

    protected override void UpdateUI()
    {
        if (uiCtrl == null) return;
        PlayerModel currentPlayer = uiCtrl.GetCurrentPlayerModel();
        if (currentPlayer == null) return;
        remainStepsText.text = currentPlayer.RemainSteps.ToString();
        targetStationNameText.text = uiCtrl.GetDestStationName();
        targetStepsText.text = uiCtrl.GetTargetSteps().ToString();
    }
}
