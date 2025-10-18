using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Upgrade
    private readonly Dictionary<UpgradeType, float> _stats = new();

    public float GetStatValue(UpgradeType type)
    {
        if (_stats.TryGetValue(type, out float value))
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }

    public void UpdateStat(UpgradeType type, float value)
    {
        if (_stats.ContainsKey(type))
        {
            _stats[type] = value;
        }
        else
        {
            _stats.Add(type, value);
        }
    }
    #endregion

    #region Money
    public long _money;

    public long Money
    {
        get { return _money; }
        //set { _money = value; }
    }
    #endregion
    
    public long useMoney { get; private set; } // 전체 사용 돈
    
    public void UpdateMoney(long money)
    {
        if (money < 0)
        {
            useMoney -= money; // 음수니 빼서 양수로
        }

        _money += money;
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
