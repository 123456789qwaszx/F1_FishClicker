using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Game Data/Upgrade Database")]
public class UpgradeDatabase : ScriptableObject
{
    public List<UpgradeData> upgradeList = new List<UpgradeData>();
}