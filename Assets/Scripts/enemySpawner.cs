using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemies; //Change to array for different types of enemies
    [SerializeField] Transform spawnLocation;

    public void SpawnEnemy()
    {
        Instantiate(enemies, spawnLocation.position, spawnLocation.rotation);
    }
}
