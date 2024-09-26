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
    [SerializeField] GameObject menuSettings;
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
    [Range(5, 40)][SerializeField, Tooltip("Max amount of enemies on the map at a time.")] int enemyMaxSpawn = 30; //Max amount of enemies on the map at a time.
    [Range(0.5f, 5)][SerializeField] float enemySpawnTimer = 3.0f;
    [Range(1, 20)][SerializeField] float enemyBaseSpawnCount = 2.0f;
    [Range(0.1f, 1)][SerializeField] float enemyWaveSpawnCurve = 0.2f;
    [Range(50, 200)][SerializeField] float enemyMoneyBase = 100.0f;
    [Range(0.05f, 0.2f)][SerializeField] float enemyMoneyCurve = 0.1f;
    [Range(3, 10)][SerializeField] float buyPhaseTimer = 30.0f; //Time in seconds

    [Header("Enemy Variables")]
    [SerializeField] GameObject[] enemies;
    [Range(1, 5)][SerializeField] float[] enemyWeight;
    [Range(1, 5)][SerializeField] int[] enemyMinWave;

    [SerializeField] int currentWave = 1;
    public int GetCurrentWave() { return currentWave; }

    List<int> enemySpawnMap = new List<int>();
    public enemySpawner[] enemySpawners;

    int enemyCount = 0; //How many enemies on the map
    int enemyRemaining; //How many enemies until wave is over
    public int GetEnemiesRemaining() { return enemyRemaining; }

    int enemyKilled;
    public int GetEnemiesKilled() { return enemyKilled; }

    int enemyValue = 100;
    int playerMoney = 0;
    public int GetPlayerMoney() { return playerMoney; }
    public void SpendMoney(int amount)
    {
        playerMoney -= amount;
        playerInterface.UpdatePlayerInterface();
    }

    //Game Phases
    enum GamePhase { BUY, COMBAT };
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

        CalculateWaveAmount();
        CalculateMoneyAmount();

        enemySpawners = GameObject.FindObjectsByType<enemySpawner>(FindObjectsSortMode.None);
        if (enemySpawners.Length == 0) canSpawn = false;

        menuSettings.SetActive(true);//Activate settings menu to load settings, it auto closes on first open
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
            else if (menuActive == menuSettings)
            {
                ToggleSettings();
            }
            else if (menuActive == menuPause || menuActive == menuShop)
            {
                StateUnpause();
            }
        }

        if (Input.GetButtonDown("Shop") && currentPhase == GamePhase.BUY)
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuShop;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuShop)
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
    public void UpdateGameGoal(Vector3 enemyPosition)
    {
        enemyCount--;
        enemyRemaining--;
        enemyKilled++;

        playerMoney += enemyValue;
        //Only need to update the player interface whenever something changes
        //no need to do every frame
        playerInterface.UpdatePlayerInterface();

        //Check for ammo drop chance
        TryDropAmmo(enemyPosition);

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

    public void ToggleSettings()
    {
        StatePause();
        menuActive.SetActive(false);

        if (menuActive == menuSettings)
        {
            menuActive = menuPause;
            menuActive.SetActive(true);
        }
        else
        {
            menuActive = menuSettings;
            menuActive.SetActive(true);
        }
    }

    public bool CanBuy()
    {
        if (currentPhase == GamePhase.BUY)
            return true;
        return false;
    }

    //Calculate how many enemies for current wave
    void CalculateWaveAmount()
    {
        if (currentWave >= 75)
        {
            enemyRemaining = 350 + (currentWave - 75) * 10;
            return;
        }
        float _waveModifier = enemyWaveSpawnCurve / Mathf.Log10((float)currentWave + 1); //Log modifier for enemy spawn count
        float _waveMultiplier = Mathf.Pow(2, _waveModifier * (((float)currentWave - 1) / 2)); //Using modifier to create spawn count multiplier

        float _enemyRemaining = enemyBaseSpawnCount * _waveMultiplier;


        enemyRemaining = (int)_enemyRemaining;
        CalculateEnemyWaveMap();
    }

    //Generate Enemies for this wave
    void CalculateEnemyWaveMap()
    {
        float totalWeight = 0.0f;
        for (int i = 0; i < enemyWeight.Length; i++)
        {
            if (enemyMinWave[i] > currentWave) { continue; }
            totalWeight += enemyWeight[i];
        }

        float amount;
        enemySpawnMap.Clear();
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemyMinWave[i] > currentWave) { continue; }
            amount = (enemyWeight[i] / totalWeight) * enemyRemaining;
            for (int n = 0; n < amount; n++)
            {
                enemySpawnMap.Add(i);
            }
        }
        enemyRemaining = enemySpawnMap.Count;
    }

    void CalculateMoneyAmount()
    {
        float _waveModifier = enemyMoneyCurve / Mathf.Log10(currentWave + 1); //Log modifier for enemy spawn count
        float _waveMultiplier = Mathf.Pow(2, _waveModifier * ((currentWave - 1) / 2)); //Using modifier to create spawn count multiplier

        float _enemyValue = enemyMoneyBase * _waveMultiplier;

        enemyValue = (int)_enemyValue;
    }

    [Header("Ammo Pickup Settings")]
    public GameObject ammoPickUpPrefab;
    public float ammoPickUpSpawnRadius = 2.0f;

    void TryDropAmmo(Vector3 enemyPosition)
    {
        float dropChance = 0f;

        if (enemyKilled >= 40)
        {
            dropChance = 10f;
        }
        else if (enemyKilled >= 30)
        {
            dropChance = 8f;
        }
        else if (enemyKilled >= 20)
        {
            dropChance = 5f;
        }

        float randomValue = Random.Range(0f, 100f);

        if (randomValue < dropChance)
        {
            Vector3 spawnPosition = enemyPosition + Random.insideUnitSphere * ammoPickUpSpawnRadius;
            Instantiate(ammoPickUpPrefab, spawnPosition, Quaternion.identity);
        }
    }

    [Header("Health Pickup Settings")]
    public GameObject healthPickUpPreFab;
    public Transform healthPickUpSpawnLocation;
    public int wavesUntilHealthRespawn = 2;  // Number of waves until health pickup respawns

    private bool healthPickUpAvailable = true;

    void SpawnHealthPickUp()
    {
        if (healthPickUpAvailable && healthPickUpPreFab != null && healthPickUpSpawnLocation != null)
        {
            Instantiate(healthPickUpPreFab, healthPickUpSpawnLocation.position, healthPickUpSpawnLocation.rotation);
            healthPickUpAvailable = false;
        }
    }

    public bool RespawnHealthPickup()
    {
         // Logic to check if the health pickup should respawn based on the wave number
         bool isMultipleOfWave = (currentWave % wavesUntilHealthRespawn == 0);


         // Simplified logic: only check the wave condition
         bool shouldRespawn = isMultipleOfWave;
    
    return shouldRespawn;
    }

    // Handle health pickup respawn
    public void HandleHealthPickupRespawn(HealthPickup healthPickup)
    {
        StartCoroutine(RespawnHealthPickupCoroutine(healthPickup));
    }

    private IEnumerator RespawnHealthPickupCoroutine(HealthPickup healthPickup)
    {
        float respawnTime = 10f;  // Respawn time delay
        yield return new WaitForSeconds(respawnTime);

        if (RespawnHealthPickup())
        {
            healthPickup.ResetPickup();  // Reactivate the health pickup
            healthPickUpAvailable = true;  // Set the pickup as available
        }
    }

    //Phase change handler
    IEnumerator NextWavePhase()
    {
        currentPhase = GamePhase.BUY;

        yield return new WaitForSeconds(buyPhaseTimer);

        currentWave++;

        if (RespawnHealthPickup())
        {
            SpawnHealthPickUp(); // Ensure to call the method to spawn the pickup
        }

        CalculateWaveAmount();
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
        List<enemySpawner> inRangeSpawners = new List<enemySpawner>();
        foreach (enemySpawner s in enemySpawners)
        {
            if (s.InRange())
                inRangeSpawners.Add(s);
        }

        if (enemyCount < enemyMaxSpawn && enemyRemaining > enemyCount)
        {
            enemyCount++;
            int randSpawner = Random.Range(0, inRangeSpawners.Count);
            int randEnemy = Random.Range(0, enemySpawnMap.Count);
            inRangeSpawners[randSpawner].SpawnEnemy(enemies[enemySpawnMap[randEnemy]]);
            enemySpawnMap.RemoveAt(randEnemy);
        }

        yield return new WaitForSeconds(enemySpawnTimer);
        canSpawn = true;
    }
}
