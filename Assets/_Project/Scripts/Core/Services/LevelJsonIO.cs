using System.IO;
using UnityEngine;

public static class LevelJsonIO
{
    public static void SaveLevelToJson(LevelData level, string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        var json = JsonUtility.ToJson(level, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Saved level json to: " + filePath);
    }

    public static LevelData LoadLevelFromJson(string filePath)
    {
        if (!File.Exists(filePath)) return null;
        var json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<LevelData>(json);
    }
}