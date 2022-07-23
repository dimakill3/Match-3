using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void LoadScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
