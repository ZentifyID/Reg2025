using System;
using System.Linq;

[Serializable]
public class UserData
{
    public int lastUnlockedLevel = 0;
    public int coins;
    public int lastPlayedLevel = 0;

    // UNIX time seconds когда можно снова получить награду
    public long nextFreeCoinsTime;
    public long nextVideoCoinsTime;
    public long nextBackgroundVideoTime;

    public string[] ownedBackgroundIds;
    public string selectedBackgroundId;

    public bool IsOwned(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        return ownedBackgroundIds != null && ownedBackgroundIds.Contains(id);
    }

    public void AddOwned(string id)
    {
        if (IsOwned(id)) return;

        if (ownedBackgroundIds == null)
            ownedBackgroundIds = new[] { id };
        else
            ownedBackgroundIds = ownedBackgroundIds.Concat(new[] { id }).ToArray();
    }
}
