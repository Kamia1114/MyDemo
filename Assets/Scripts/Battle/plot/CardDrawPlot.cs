using UnityEngine;

/// <summary>
/// 抽卡地块，玩家进入时可以抽取卡片
/// </summary>
public class CardDrawPlot : Plot
{
    [Header("抽卡设置")]
    [Tooltip("卡片池")]
    [SerializeField] private CardPool cardPool;
    
    [Tooltip("每次抽取的卡片数量")]
    [SerializeField] private int cardsToDraw = 1;

    /// <summary>
    /// 玩家进入抽卡地块的行为
    /// </summary>
    public override void OnPlayerEnter(BasePlayer player)
    {
        if (cardPool != null)
        {
            var drawnCards = cardPool.DrawCards(cardsToDraw);
            foreach (var card in drawnCards)
            {
                card.ApplyEffect(player);
                UIManager.Instance.ShowCardDrawn(player, card);
            }
        }
    }
}
