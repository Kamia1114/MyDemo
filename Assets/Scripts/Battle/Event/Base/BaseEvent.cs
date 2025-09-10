// 卡牌事件状态接口
public interface ICardEventState
{
    void Enter(BaseEvent cardEvent);
    void Update(BaseEvent cardEvent);
    void Exit(BaseEvent cardEvent);
}

// 基础事件类（所有卡牌事件的基类）
public abstract class BaseEvent
{
    public int CardId { get; protected set; }
    public ICardEventState CurrentState { get; protected set; }
    public bool IsActive { get; protected set; } = true;

    protected BaseEvent(int cardId)
    {
        CardId = cardId;
    }

    public void ChangeState(ICardEventState newState)
    {
        CurrentState?.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    public virtual void Update()
    {
        if (IsActive)
        {
            CurrentState?.Update(this);
        }
    }

    public virtual void End()
    {
        IsActive = false;
        CurrentState?.Exit(this);
        CurrentState = null;
    }
}