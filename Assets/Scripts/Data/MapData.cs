using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapData
{
    public int id;
    public string mapName;
    public string region;
    public List<string> fishPool; // CSV에서 "Clownfish|BlueTang" → List<string>
    public string backgroundSprite;
    public string description;
}