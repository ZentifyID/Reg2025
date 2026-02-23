using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorButton : MonoBehaviour
{
    [SerializeField] private string levelEditorSceneName = "LevelEditor";

    public void Editor()
    {
        SceneManager.LoadScene(levelEditorSceneName);
    }
}