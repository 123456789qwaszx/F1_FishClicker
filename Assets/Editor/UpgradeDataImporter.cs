using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UpgradeDataImporter
{
    private const string CsvPath = "Assets/DataTables/UpgradeData.csv";
    private const string AssetPath = "Assets/Resources/UpgradeDatabase.asset";

    [MenuItem("Tools/Import/Upgrade CSV → ScriptableObject")]
    public static void ImportUpgradeData()
    {
        var csvData = CSVImporter.ReadCSV(CsvPath);
        if (csvData == null || csvData.Count == 0)
        {
            Debug.LogError("UpgradeDataImporter: No data found in CSV.");
            return;
        }

        UpgradeDatabase database = ScriptableObject.CreateInstance<UpgradeDatabase>();

        foreach (var row in csvData)
        {
            // UpgradeData 객체 생성
            UpgradeData upgrade = new UpgradeData();

            // UpgradeType 생성 및 null-safe 초기화
            string statType = row.ContainsKey("statType") ? row["statType"] : "";
            string effectStr = row.ContainsKey("effectType") ? row["effectType"] : "";
            UpgradeEffectType effect = UpgradeEffectType.Additive;

            if (!string.IsNullOrEmpty(effectStr))
            {
                if (!Enum.TryParse(effectStr, out effect))
                {
                    Debug.LogWarning($"Unknown effectType '{effectStr}' for statType '{statType}', defaulting to Additive.");
                    effect = UpgradeEffectType.Additive;
                }
            }

            upgrade.type = new UpgradeData.UpgradeType(statType, effect);

            // 숫자 값 파싱
            upgrade.level = ParseInt(row, "level", 0);
            upgrade.baseStatValue = ParseLong(row, "baseStatValue", 0);
            upgrade.valueIncrease = ParseLong(row, "valueIncrease", 0);
            upgrade.baseCost = ParseLong(row, "baseCost", 0);
            upgrade.costIncrease = ParseLong(row, "costIncrease", 0);

            database.upgradeList.Add(upgrade);
        }

        // ScriptableObject 저장
        AssetDatabase.CreateAsset(database, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Upgrade data imported successfully! ({database.upgradeList.Count} entries)");
    }

    private static int ParseInt(Dictionary<string, string> row, string key, int defaultValue)
    {
        return row.ContainsKey(key) && int.TryParse(row[key], out int result) ? result : defaultValue;
    }

    private static long ParseLong(Dictionary<string, string> row, string key, long defaultValue)
    {
        return row.ContainsKey(key) && long.TryParse(row[key], out long result) ? result : defaultValue;
    }
}
