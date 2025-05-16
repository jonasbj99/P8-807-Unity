using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections; // Required for the new Input System

public class CustomSceneManager : MonoBehaviour
{
    FadeInOut fade;


    private static CustomSceneManager _instance;

    public static CustomSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CustomSceneManager");
                obj.AddComponent<CustomSceneManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        fade = FindFirstObjectByType<FadeInOut>(); // Find the FadeInOut component in the scene
        fade.FadeOut(); // Start fading out after changing the scene
    }

    private void Update()
    {
        var keyboard = Keyboard.current; // Access the keyboard using the new Input System

        if (keyboard.digit7Key.wasPressedThisFrame) // Check if the "1" key was pressed
        {
            NextScene();
        }
    }

    public IEnumerator SceneChanger()
    {
        fade.FadeIn(); // Start fading in before changing the scene
        yield return new WaitForSeconds(fade.TimeToFade);

        // Get the current scene's build index and calculate the next scene's index
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // Load the next scene
        SceneManager.LoadScene(nextSceneIndex, LoadSceneMode.Single);

    }

    public void NextScene()
    {
        StartCoroutine(SceneChanger());
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

