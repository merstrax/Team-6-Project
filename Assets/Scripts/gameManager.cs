using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    // User Interface Variables
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    GameObject menuActive;

    public GameObject damagePanel;

    //Game Objects
    public GameObject player;
    public PlayerController playerScript;

    int currentWave;
    int enemyCount; //How many enemies on the map
    int enemyRemaining; //How many enemies until wave is over

    //Wave variables
    [SerializeField] int enemyMaxSpawn = 30; //Max amount of enemies on the map at a time.
    [SerializeField] float enemySpawnTimner = 3.0f;
    [SerializeField] float enemyBaseSpawnCount = 10.0f;
    [SerializeField] float enemyWaveSpawnCurve = 0.2f;
    [SerializeField] float enemyMoneyBase = 100.0f;
    [SerializeField] float enemyMoneyCurve = 0.1f;

    [SerializeField] float buyPhaseTimer = 30.0f; //Time in seconds

    //Game Phases
    enum GamePhase{BUY, COMBAT};
    GamePhase currentPhase = GamePhase.BUY;

    //Game State and default settings
    float timeScaleOrig;
    public bool isPaused;
    private bool canSpawn;

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
    public void UpdateGameGoal(int amount)
    {
        enemyRemaining += amount;

        if(enemyRemaining <= 0)
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
        menuActive.SetActive(isPaused);
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
        float _waveModifier = enemyWaveSpawnCurve / Mathf.Log10(currentWave + 1); //Log modifier for enemy spawn count
        float _waveMultiplier = Mathf.Pow(2, _waveModifier * ((currentWave - 1) / 2)); //Using modifier to create spawn count multiplier

        float _enemyRemaining = enemyBaseSpawnCount * _waveMultiplier;

        enemyRemaining = (int)_enemyRemaining;
    }

    //Phase change handler
    IEnumerator NextWavePhase()
    {
        currentPhase = GamePhase.BUY;

        yield return new WaitForSeconds(buyPhaseTimer);

        currentWave++;
        CalclulateWaveAmount();
        currentPhase = GamePhase.COMBAT;
        canSpawn = true;
    }

    //Enemy spawn handler
    IEnumerator SpawnEnemy()
    {
        canSpawn = false;

        if(enemyCount >= enemyMaxSpawn)
        {
            //Spawn an enemy
        }

        yield return new WaitForSeconds(enemySpawnTimner);
        canSpawn = true;
    }
}
