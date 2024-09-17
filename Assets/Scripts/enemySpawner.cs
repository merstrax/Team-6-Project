using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] Transform spawnLocation;

    bool playerInRange;

    public void SpawnEnemy(GameObject enemy)
    {
        Instantiate(enemy, spawnLocation.position, spawnLocation.rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public bool InRange()
    { 
        return playerInRange;
    }

}
