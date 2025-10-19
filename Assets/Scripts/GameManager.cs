using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Money
    private long _money;
    
    public long Money
    {
        get { return _money; }
    }
    public void UpdateMoney(long money)
    {
        if (money < 0)
        {
            useMoney -= money; // 음수니 빼서 양수로
        }

        _money += money;
    }
    #endregion
    
    
    public int FishCaughtCount { get; private set; } // 잡은 물고기 숫자
    
    public long useMoney { get; private set; } // 전체 사용 돈
}
