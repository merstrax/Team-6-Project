using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class buttonMainFunctions : MonoBehaviour
{
    [SerializeField] TMP_InputField senseField;

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

    public void changeSensitivity()
    {
        Camera.main.GetComponent<cameraController>().ChangeSensitivity(int.Parse(senseField.text));
    }

}
