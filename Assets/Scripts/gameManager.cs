using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    // the different menus
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public GameObject damagePanel;
    public GameObject player;
    public PlayerController playerScript;

    GameObject menuActive;
    int enemyCount;
    float timeScaleOrig;
    public bool isPaused;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // pausing the game with Esc
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuPause.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }
    }
    
    // stops time and shows the cursor
    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // unpauses the game and closes the pause menu
    public void StateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }

    // pops up the win menu if all of the enemies are dead
    public void UpdateGameGoal(int amount)
    {
        enemyCount += amount;

        if(enemyCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(isPaused);
        }
    }

    // pops up the lose menu if the player loses
    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(isPaused);
    }
}
