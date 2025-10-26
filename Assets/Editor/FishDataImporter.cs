using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FishDataImporter
{
    private const string CsvPath = "Assets/DataTables/FishData.csv";
    private const string AssetPath = "Assets/Resources/FishDatabase.asset";

    [MenuItem("Tools/Import/Fish CSV → ScriptableObject")]
    public static void ImportFishData()
    {
        var csvData = CSVImporter.ReadCSV(CsvPath);
        if (csvData == null || csvData.Count == 0)
        {
            Debug.LogError("FishDataImporter: No data found in CSV.");
            return;
        }

        FishDatabase database = ScriptableObject.CreateInstance<FishDatabase>();

        foreach (var row in csvData)
        {
            FishData fish = new FishData();
            fish.id = int.Parse(row["id"]);
            fish.fishName = row["fishName"];
            fish.rarity = row["rarity"];
            fish.baseValue = long.Parse(row["baseValue"]);
            fish.spritePath = row["spritePath"];
            fish.description = row["description"];
            fish.region = row["region"];

            database.fishList.Add(fish);
        }

        AssetDatabase.CreateAsset(database, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Fish data imported successfully! ({database.fishList.Count} entries)");
    }
}