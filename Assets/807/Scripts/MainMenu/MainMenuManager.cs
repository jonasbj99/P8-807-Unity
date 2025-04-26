using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public UIFader levelSelectFader;

    public void StartGame()
    {
        SceneManager.LoadScene("Scenario1_Apartment");
    }

    public void OpenLevelSelect()
    {
        levelSelectFader.FadeIn();
    }

    public void CloseLevelSelect()
    {
        levelSelectFader.FadeOut();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

