using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    // time before drop disappears 
    [SerializeField] float lifeSpan = 15f;

    private void Start()
    {
        //Start Coroutine to handle automatic destruction 
        StartCoroutine(DestroyAfterTime());
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Get the WeaponHandler component
            weaponHandler weaponHandler = other.GetComponent<PlayerController>().GetWeapon(); 

            if (weaponHandler != null)
            {
                // Set the player's current ammo to max ammo
                weaponHandler.SetAmmoToMax();

                //destroyed once picked up
                Destroy(gameObject); 
            }
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeSpan);

        // Destroys Drop after after lifespan
        Destroy(gameObject); 
    }
}
