namespace Core.Enum
{

	public enum UseTypeEnum 
	{
		骰子数量, 
		骰子点数, 
		强制投骰子, 
		传送, 
		资金增减, 
		扫把星增减, 
		召唤物抽取驱逐, 
		障碍物放置, 
		卡牌增减, 
		房产收益增减, 
		骰子点数增减, 
		特殊房产增减, 
		无效攻击, 
		卡牌禁用, 
		睡眠, 
		直线移动, 
		重置目的地, 
		考题, 
		无视障碍物, 
		效果延时减时, 
		木头人, 
		卡牌保险柜, 
		去旅游, 
		故地重游, 
		烟花爆竹, 
		烫手山芋, 
		白猫警长, 
		福星高照, 
		说一不二, 
		金刚不坏, 
		鸡鸣狗盗 
	}

	public enum TargetTypeEnum 
	{
		自己, 
		别人, 
		除自己外所有, 
		所有, 
		强制指定 
	}

	public enum CountTypeEnum 
	{
		回合, 
		次数, 
		随机持续回合, 
		随机次数, 
		步数后用尽 
	}

	public enum GridTypeEnum 
	{
		城市, 
		金钱, 
		卡牌, 
		彩票, 
		传送, 
		商店, 
		事件 
	}

	public enum CardTypeEnum 
	{
		攻击, 
		特殊, 
		移动, 
		负面效果 
	}
}