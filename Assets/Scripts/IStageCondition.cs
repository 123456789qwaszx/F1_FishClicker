public interface IStageCondition
{
    bool IsSatisfied { get; }
    void Initialize(); // 필요 시 초기화
    void Subscribe();  // 이벤트 구독
    void Unsubscribe();
}