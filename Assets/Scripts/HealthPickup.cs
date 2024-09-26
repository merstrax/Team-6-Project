using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Range(0, 15)][SerializeField] int healthBoostAmount;

    private bool isPickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            // Ensure playerController is found
            if (playerController != null)
            {
                // Heal the player by the health boost amount
                playerController.Heal(healthBoostAmount);

                // Mark as picked up and deactivate the pickup object
                isPickedUp = true;
                gameObject.SetActive(false);

                // Notify the gameManager to handle respawn
                gameManager.instance.HandleHealthPickupRespawn(this); 
            }
        }
    }

    // Reset the pickup's state
    public void ResetPickup()
    {
        isPickedUp = false;
        gameObject.SetActive(true);
    }
}