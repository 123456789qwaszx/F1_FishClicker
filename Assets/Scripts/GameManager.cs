using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum GamePhase
{
    // === DAY SCENE ===
    EditStation, // 배치 편집
    Day,         // 일상
    SelectMenu, // 메뉴 선택창 켰을 때
    Shop,        // 상점
    
    // === MAIN SCENE ===
    Opening,     // 손님 스폰 전
    Operation,   // 실제 영업중
    Closing,     // 영업 마감
    
    // === 특수 상태 ===
    Dialogue,
    Paused,
    GameOver,
    Loading,
    None,
}
