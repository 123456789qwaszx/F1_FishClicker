public class KillCountCondition : IStageCondition
{
    private int requiredCount;
    private bool satisfied;

    public bool IsSatisfied => satisfied;

    public KillCountCondition(int requiredCount)
    {
        this.requiredCount = requiredCount;
    }

    public void Initialize() => satisfied = false;

    public void Subscribe()
    {
        //EventManager.Instance.AddListener(EEventType.OnEnemyDefeated, OnEnemyDefeated);
    }

    public void Unsubscribe()
    {
        //EventManager.Instance.RemoveListener(EEventType.OnEnemyDefeated, OnEnemyDefeated);
    }

    private void OnEnemyDefeated(object payload)
    {
        //if(++Player.Instance.KillCount >= requiredCount)
            satisfied = true;
    }
}