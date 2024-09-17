using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class buttonMainFunctions : MonoBehaviour
{
    public void settings()
    {
        gameManager.instance.ToggleSettings();
    }

    public void mainMenuSettings(GameObject menuSettings)
    {
        menuSettings.SetActive(!menuSettings.activeSelf);
    }

    public void resume()
    {
        gameManager.instance.StateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.StateUnpause();
    }

    public void quit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void startgame()
    {
        SceneManager.LoadScene("Game Scene");
    }
}
