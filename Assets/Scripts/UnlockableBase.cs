using UnityEngine;

public enum EUnlockedState
{
    Hidden,
    Unlockable,
    Unlocked
}

public class UnlockableStateData
{
    public EUnlockedState State = EUnlockedState.Hidden;
    //public int RequiredMoney;
    public int RequiredLevel;
}

public class UnlockableBase : MonoBehaviour
{
	public UI_UnlockButton UI_UnlockButton;
	private UnlockableStateData _data = new UnlockableStateData();


    void Awake()
    {
        //EventManager.Instance.AddEvent(EEventType.ScheduleEnded, SetInfo);
        //SetUnlockedState(_data.State);
    }

    void OnEnable()
    {
        if (gameObject.activeInHierarchy == false)
            return;
        //     
        // if (State == EUnlockedState.Unlocked)
        //     return;

        UI_UnlockButton.TryUnlock();
    }


    // public void SetData(ItemData item)
    // {
    //     switch (item.Grade)
    //     {
    //         case ItemGrade.None:
    //             _data.State = EUnlockedState.Unlocked;
    //             break;
    //         case ItemGrade.Normal:
    //             _data.State = EUnlockedState.Unlockable;
    //             //_data.RequiredMoney = 50;
    //             _data.RequiredLevel = item.UnLockLevel;
    //             break;
    //         case ItemGrade.Rare:
    //             _data.State = EUnlockedState.Unlockable;
    //             //_data.RequiredMoney = 100;
    //             _data.RequiredLevel = item.UnLockLevel;
    //             break;
    //         case ItemGrade.Unique:
    //             _data.State = EUnlockedState.Unlockable;
    //             //_data.RequiredMoney = 150;
    //             _data.RequiredLevel = item.UnLockLevel;
    //             break;
    //         case ItemGrade.Legendary:
    //             _data.State = EUnlockedState.Unlockable;
    //             //_data.RequiredMoney = 200;
    //             _data.RequiredLevel = item.UnLockLevel;
    //             break;
    //     }
    //}

	// public void SetInfo()
 //    {
 //        UI_UnlockButton.RefreshUI(_data.RequiredLevel);
 //    }
 //
 //
 //    EUnlockedState State
	// {
	// 	get { return _data.State; }
	// 	set { _data.State = value; }
	// }
 //
	// public bool IsUnlocked => State == EUnlockedState.Unlocked;
 //    
 //
	// public void SetUnlockedState(EUnlockedState state)
 //    {
 //        State = state;
 //
 //        if (state == EUnlockedState.Hidden)
 //        {
 //            gameObject.SetActive(false);
 //            UI_UnlockButton.gameObject.SetActive(false);
 //        }
 //        else if (state == EUnlockedState.Unlockable)
 //        {
 //            gameObject.SetActive(false);
 //            UI_UnlockButton.gameObject.SetActive(true);
 //        }
 //        else
 //        {
 //            gameObject.SetActive(true);
 //            UI_UnlockButton.gameObject.SetActive(false);
 //        }
 //    }
}
