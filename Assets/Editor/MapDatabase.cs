using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDatabase", menuName = "Game Data/Map Database")]
public class MapDatabase : ScriptableObject
{
    public List<MapData> mapList = new List<MapData>();
}