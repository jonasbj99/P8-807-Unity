using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Required for the new Input System

public class CustomSceneManager : MonoBehaviour
{
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

    private void Update()
    {
        var keyboard = Keyboard.current; // Access the keyboard using the new Input System

        if (keyboard.digit1Key.wasPressedThisFrame) // Check if the "1" key was pressed
        {
            SceneChanger("XR RIG Setup Scene");
        }
        else if (keyboard.digit2Key.wasPressedThisFrame) // Check if the "2" key was pressed
        {
            SceneChanger("ShaderTestScene");
        }
    }

    public void SceneChanger(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}

