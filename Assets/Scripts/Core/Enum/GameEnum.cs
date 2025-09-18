namespace Core.Enum
{
	public enum SceneState
	{
		Main,
		Battle
	}

	public enum QualityLevel
	{
		N,
		R,
		SR,
		SSR
	}


	public enum GridEventEnum
	{
		OnGridClicked, // 玩家点击格子事件
	}

	public enum PlayerEventEnum
	{
		OnPlayerLuck, // 玩家幸运状态事件
	}
	
	// 触发时机（用于被动卡牌）
	// public enum TriggerTimingEnum {
	// 	TurnStart,   // 回合开始
	// 	TurnEnd,     // 回合结束
	// 	StepMove,    // 移动时
	// 	MoneyChange  // 金钱变化时
	// 	// 可扩展其他时机
	// }

}