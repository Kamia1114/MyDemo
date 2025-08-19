using UnityEngine;

/// <summary>
/// 加钱地块，玩家进入时获得金钱
/// </summary>
public class MoneyPlot : MoneyModificationPlot
{
    /// <summary>
    /// 玩家进入加钱地块的行为
    /// </summary>
    public override void OnPlayerEnter(Player player)
    {
        if (this.PlotType == PlotType.MoneyAdd)
        {
            int amount = CalculateAmount(player);
            player.AddMoney(amount);
            UIManager.Instance.ShowMoneyAdded(player, amount);
        }
        else if (this.PlotType == PlotType.MoneyDeduct)
        {
            int amount = CalculateAmount(player);
            // 确保不会扣除负数
            amount = Mathf.Abs(amount);
            // 如果钱不够，扣到0为止
            int actualDeduction = Mathf.Min(amount, player.Money);
            player.SubtractMoney(actualDeduction);
            UIManager.Instance.ShowMoneyDeducted(player, actualDeduction);
        } 
        else
        {
            Debug.LogWarning("MoneyPlot: Unsupported PlotType for MoneyPlot.");
        }

    }

    /// <summary>
    /// 计算实际增加的金钱数量
    /// </summary>
    private int CalculateAmount(BasePlayer player)
    {
        if (isPercentage)
        {
            return Mathf.RoundToInt(player.Money * (moneyAmount / 100f));
        }
        return moneyAmount;
    }
}
