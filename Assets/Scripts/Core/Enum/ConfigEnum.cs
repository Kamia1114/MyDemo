namespace Core.Enum
{

	public enum UseTypeEnum 
	{
		Active, 
		Passive, 
		All 
	}

	public enum TargetTypeEnum 
	{
		None, 
		Self, 
		OtherOne, 
		OtherRandomOne, 
		Others, 
		All, 
		Grid 
	}

	public enum CountTypeEnum 
	{
		Time, 
		RandomTime 
	}

	public enum GridTypeEnum 
	{
		City, 
		Money, 
		Card, 
		Lottery, 
		Fly, 
		Shop, 
		Action 
	}

	public enum CardTypeEnum 
	{
		ATK, 
		Move, 
		Buff, 
		Debuff, 
		Default 
	}

	public enum TriggerTimingEnum 
	{
		Roll, 
		RoundCount, 
		TurnStart, 
		TurnEnd, 
		MoveStart, 
		MoveEnd, 
		MoneyAdd, 
		Money‌Subtract‌, 
		CompanyBuy, 
		BuyCompany 
	}

	public enum InteractionTypeEnum 
	{
		Default, 
		DiceCount, 
		FlyTo, 
		Player, 
		PlayerCard, 
		SelfCard, 
		Grid 
	}

	public enum CardUITypeEnum 
	{
		Player, 
		Card, 
		Fly 
	}

	public enum SkillTypeEnum 
	{
		DiceCount, 
		DiceNumber, 
		AddMoney, 
		CutMoney, 
		AddCard, 
		DelCard, 
		RobMoney, 
		RobCard, 
		Fly, 
		FlyTo, 
		CometLeave, 
		Robbery, 
		ClearDebt, 
		WealthGod, 
		Counter, 
		CopyCard 
	}
}