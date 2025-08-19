using UnityEngine;
/// <summary>
/// 金钱修改地块基类，用于扣钱和加钱地块的共同逻辑
/// </summary>
public abstract class MoneyModificationPlot : Plot
{
    [Header("金钱设置")]
    [Tooltip("金钱变动值")]
    [SerializeField] protected int moneyAmount;

    [Tooltip("是否按比例变动（百分比）")]
    [SerializeField] protected bool isPercentage;
}
