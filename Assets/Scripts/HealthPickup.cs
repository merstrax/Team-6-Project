using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Range(0, 30)][SerializeField] int healthBoostAmount;

    private bool isPickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isPickedUp && other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Heal(healthBoostAmount);
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