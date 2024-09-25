using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // Amount to heal the player when pick up
    [SerializeField] int healthBoostAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the playerController component
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                {
                    // Heal the player
                    playerController.Heal(healthBoostAmount);
                }
                // Destory the object
                Destroy(gameObject);
            }
        }
    }
}