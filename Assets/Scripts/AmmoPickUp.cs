using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    // time before drop disappears 
    [SerializeField] float lifeSpan = 15f;

    [SerializeField] bool isAmmo = false;
    [SerializeField] bool isMedkit = false;
    [SerializeField] int healthBoostAmount = 25;

    private void Start()
    {
        //Start Coroutine to handle automatic destruction 
        StartCoroutine(DestroyAfterTime());
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();


            if (playerController != null)
            {

                if (isAmmo)
                {
                    //Get the WeaponHandler component
                    weaponHandler weaponHandler = other.GetComponent<PlayerController>().GetWeapon();

                    // Set the player's current ammo to max ammo
                    weaponHandler.SetAmmoToMax();
                }
            }
            // Check if this is a health pickup
            if (isMedkit)
            {
                // Heal the player
                playerController.Heal(healthBoostAmount);
            }
            // Destory the item once its use
            Destroy(gameObject);
        }    
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeSpan);

        // Destroys Drop after after lifespan
        Destroy(gameObject); 
    }
}
