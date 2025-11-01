using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    public int id;
    public string mapName;
    public string region;
    public string backgroundSprite;
    public string description;
    public List<StageData> stages;
}