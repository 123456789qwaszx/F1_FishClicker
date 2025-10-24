using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVImporter
{
    public static List<Dictionary<string, string>> ReadCSV(string path)
    {
        List<Dictionary<string, string>> dataList = new();
        
        if (!File.Exists(path))
        {
            Debug.LogError($"CSVImporter: File not found at {path}");
            return dataList;
        }

        string[] lines = File.ReadAllLines(path);
        if (lines.Length <= 1)
        {
            Debug.LogError("CSVImporter: CSV file is empty or missing header");
            return dataList;
        }

        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            Dictionary<string, string> entry = new();

            for (int j = 0; j < headers.Length && j < values.Length; j++)
                entry[headers[j].Trim()] = values[j].Trim();

            dataList.Add(entry);
        }

        return dataList;
    }
}