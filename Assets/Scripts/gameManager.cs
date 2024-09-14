using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    // User Interface Variables
    [Header("User Interface")]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] playerInterface playerInterface;
    public Image playerHPBar;

    GameObject menuActive;

    public GameObject damagePanel;

    public playerInterface GetPlayerInterface() { return playerInterface; }

    //Game Objects
    [Header("Player Objects")]
    public GameObject player;
    public PlayerController playerScript;

    //Wave variables
    [Header("Wave Variables")]
    [SerializeField, Tooltip("Max amount of enemies on the map at a time.")] int enemyMaxSpawn = 30; //Max amount of enemies on the map at a time.
    [SerializeField] float enemySpawnTimer = 3.0f;
    [SerializeField] float enemyBaseSpawnCount = 2.0f;
    [SerializeField] float enemyWaveSpawnCurve = 0.2f;
    [SerializeField] float enemyMoneyBase = 100.0f;
    [SerializeField] float enemyMoneyCurve = 0.1f;

    [SerializeField] float buyPhaseTimer = 30.0f; //Time in seconds

    public enemySpawner[] enemySpawners;

    [SerializeField] int currentWave = 1;
    public int GetCurrentWave() { return currentWave; }

    int enemyCount = 0; //How many enemies on the map
    int enemyRemaining; //How many enemies until wave is over
    public int GetEnemiesRemaining() { return enemyRemaining; }

    int enemyKilled;
    public int GetEnemiesKilled() { return enemyKilled; }

    int enemyValue = 100;
    int playerMoney = 0;
    public int GetPlayerMoney() { return playerMoney; }

    //Game Phases
    enum GamePhase{BUY, COMBAT};
    GamePhase currentPhase = GamePhase.COMBAT;

    //Game State and default settings
    float timeScaleOrig;
    public bool isPaused;
    private bool canSpawn = true;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();

        CalclulateWaveAmount();
        CalculateMoneyAmount();

        enemySpawners = GameObject.FindObjectsByType<enemySpawner>(FindObjectsSortMode.None);
        if (enemySpawners.Length == 0) canSpawn = false;
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
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause || menuActive == menuShop)
            {
                StateUnpause();
            }
        }

        if(Input.GetButtonDown("Shop") && currentPhase == GamePhase.BUY)
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuShop;
                menuActive.SetActive(isPaused);
            }
            else if(menuActive == menuShop)
            {
                StateUnpause();
            }
        }

        if (canSpawn && currentPhase == GamePhase.COMBAT)
        {
            //Debug.Log("Spawn");
            StartCoroutine(SpawnEnemy());
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
    public void UpdateGameGoal()
    {
        enemyCount--;
        enemyRemaining--;
        enemyKilled++;

        playerMoney += enemyValue;
        //Only need to update the player interface whenever something changes
        //no need to do every frame
        playerInterface.UpdatePlayerInterface();

        if (enemyRemaining <= 0)
        {
            //Go to buy phase for allotted time period
            StartCoroutine(NextWavePhase());
        }
    }

    // pops up the lose menu if the player loses
    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public bool CanBuy()
    {
        if(currentPhase == GamePhase.BUY)
            return true;
        return false;
    }

    //Calculate how many enemies for current wave
    void CalclulateWaveAmount()
    {
        if(currentWave >= 75)
        {
            enemyRemaining = 350 + (currentWave - 75) * 10;
            return;
        }
        float _waveModifier = enemyWaveSpawnCurve / Mathf.Log10((float)currentWave + 1); //Log modifier for enemy spawn count
        float _waveMultiplier = Mathf.Pow(2, _waveModifier * (((float)currentWave - 1) / 2)); //Using modifier to create spawn count multiplier

        float _enemyRemaining = enemyBaseSpawnCount * _waveMultiplier;


        enemyRemaining = (int)_enemyRemaining;
    }

    void CalculateMoneyAmount()
    {
        float _waveModifier = enemyMoneyCurve / Mathf.Log10(currentWave + 1); //Log modifier for enemy spawn count
        float _waveMultiplier = Mathf.Pow(2, _waveModifier * ((currentWave - 1) / 2)); //Using modifier to create spawn count multiplier

        float _enemyValue = enemyMoneyBase * _waveMultiplier;

        enemyValue = (int)_enemyValue;
    }

    //Phase change handler
    IEnumerator NextWavePhase()
    {
        currentPhase = GamePhase.BUY;

        yield return new WaitForSeconds(buyPhaseTimer);

        currentWave++;
        CalclulateWaveAmount();
        CalculateMoneyAmount();
        //Update the player interface
        playerInterface.UpdatePlayerInterface();
        currentPhase = GamePhase.COMBAT;
        canSpawn = true;
        playerScript.PlayNewWaveSound();
    }

    //Enemy spawn handler
    IEnumerator SpawnEnemy()
    {
        canSpawn = false;

        if(enemyCount < enemyMaxSpawn && enemyRemaining > enemyCount)
        {
            enemyCount++;
            int rand = Random.Range(0, enemySpawners.Length);
            enemySpawners[rand].SpawnEnemy();
        }

        yield return new WaitForSeconds(enemySpawnTimer);
        canSpawn = true;
    }
}
