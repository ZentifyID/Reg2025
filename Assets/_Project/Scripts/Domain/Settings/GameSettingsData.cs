using System;

[Serializable]
public class GameSettingsData
{
    public int resolutionIndex;   // 0..3
    public bool fullscreen;
    public bool audioEnabled;
    public int languageIndex;     // 0 ru, 1 en

    public GameSettingsData Clone() => new GameSettingsData
    {
        resolutionIndex = resolutionIndex,
        fullscreen = fullscreen,
        audioEnabled = audioEnabled,
        languageIndex = languageIndex
    };

    public bool EqualsTo(GameSettingsData other)
    {
        if (other == null) return false;
        return resolutionIndex == other.resolutionIndex
               && fullscreen == other.fullscreen
               && audioEnabled == other.audioEnabled
               && languageIndex == other.languageIndex;
    }
}
