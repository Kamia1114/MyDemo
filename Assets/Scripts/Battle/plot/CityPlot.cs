using UnityEngine;

/// <summary>
/// 城市地块，可以购买、升级的地块类型
/// </summary>
public class CityPlot : Plot
{
    [Header("城市属性")]
    [Tooltip("城市名称")]
    [SerializeField] private string cityName;
    
    [Tooltip("购买价格")]
    [SerializeField] private int purchasePrice;
    
    [Tooltip("基础租金")]
    [SerializeField] private int baseRent;
    
    [Tooltip("房屋建设成本")]
    [SerializeField] private int houseCost;
    
    [Tooltip("当前房屋等级")]
    [SerializeField] private int houseLevel;
    
    [Tooltip("所属玩家")]
    private BasePlayer owner;

    // 属性访问器
    public string CityName => cityName;
    public int PurchasePrice => purchasePrice;
    public int CurrentRent => CalculateRent();
    public BasePlayer Owner => owner;
    public bool IsOwned => owner != null;

    /// <summary>
    /// 计算当前租金（根据房屋等级）
    /// </summary>
    /// <returns>计算后的租金</returns>
    private int CalculateRent()
    {
        // 租金随房屋等级递增
        return baseRent * (houseLevel + 1);
    }

    /// <summary>
    /// 购买地块
    /// </summary>
    /// <param name="buyer">购买者</param>
    /// <returns>是否购买成功</returns>
    public bool PurchasePlot(BasePlayer buyer)
    {
        if (IsOwned || buyer.Money < purchasePrice)
            return false;

        owner = buyer;
        buyer.SubtractMoney(purchasePrice);
        return true;
    }

    /// <summary>
    /// 升级房屋
    /// </summary>
    /// <returns>是否升级成功</returns>
    public bool UpgradeHouse()
    {
        if (!IsOwned || owner.Money < houseCost || houseLevel >= 5)
            return false;

        houseLevel++;
        owner.SubtractMoney(houseCost);
        return true;
    }

    /// <summary>
    /// 玩家进入城市地块的行为
    /// </summary>
    public override void OnPlayerEnter(BasePlayer player)
    {
        if (owner == null)
        {
            // 地块未被购买，提示购买
            UIManager.Instance.ShowPurchasePrompt(this, player);
        }
        else if (owner != player)
        {
            // 支付租金给地块所有者
            player.SubtractMoney(CurrentRent);
            owner.AddMoney(CurrentRent);
            UIManager.Instance.ShowRentPayment(player, owner, CurrentRent);
        }
    }
}
