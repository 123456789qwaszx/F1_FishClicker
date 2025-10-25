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
            UpgradeData upgrade = new UpgradeData();

            // Enum 파싱 (string → UpgradeType, UpgradeEffectType)
            if (System.Enum.TryParse(row["statType"], out UpgradeType parsedType))
                upgrade.statType = parsedType;
            else
                upgrade.statType = UpgradeType.None;

            if (System.Enum.TryParse(row["effectType"], out UpgradeEffectType parsedEffect))
                upgrade.effectType = parsedEffect;
            else
                upgrade.effectType = UpgradeEffectType.Additive;

            // 숫자 변환
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
        return int.TryParse(row[key], out var result) ? result : defaultValue;
    }

    private static long ParseLong(Dictionary<string, string> row, string key, long defaultValue)
    {
        return long.TryParse(row[key], out var result) ? result : defaultValue;
    }
}
