using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    public void Play()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
