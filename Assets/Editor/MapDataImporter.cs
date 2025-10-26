using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapDataImporter
{
    private const string CsvPath = "Assets/DataTables/MapData.csv";
    private const string AssetPath = "Assets/Resources/MapDatabase.asset";

    [MenuItem("Tools/Import/Map CSV → ScriptableObject")]
    public static void ImportMapData()
    {
        var csvData = CSVImporter.ReadCSV(CsvPath);
        if (csvData == null || csvData.Count == 0)
        {
            Debug.LogError("MapDataImporter: No data found in CSV.");
            return;
        }

        MapDatabase database = ScriptableObject.CreateInstance<MapDatabase>();

        foreach (var row in csvData)
        {
            MapData map = new MapData();
            map.id = int.Parse(row["id"]);
            map.mapName = row["mapName"];
            map.region = row["region"];
            map.backgroundSprite = row["backgroundSprite"];
            map.description = row["description"];

            database.mapList.Add(map);
        }

        AssetDatabase.CreateAsset(database, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Map data imported successfully! ({database.mapList.Count} entries)");
    }
}