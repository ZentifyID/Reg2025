using System.IO;
using UnityEngine;

public class UserDataStorage
{
    private const string FileName = "UserData.json";

    private string PathToFile => System.IO.Path.Combine(Application.persistentDataPath, FileName);

    public UserData LoadOrCreate()
    {
        if (!File.Exists(PathToFile))
            return new UserData { coins = 0 };

        try
        {
            string json = File.ReadAllText(PathToFile);
            var data = JsonUtility.FromJson<UserData>(json);
            return data ?? new UserData { coins = 0 };
        }
        catch
        {
            return new UserData { coins = 0 };
        }
    }

    public void Save(UserData data)
    {
        File.WriteAllText(PathToFile, JsonUtility.ToJson(data, true));
    }
}
