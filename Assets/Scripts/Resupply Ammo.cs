using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResupplyAmmo : MonoBehaviour
{
    [Range(0, 500)][SerializeField] int ammoCost;
    [Range(10, 100)][SerializeField] int ammoAmount;
    [SerializeField] TextMeshProUGUI ammoPromptText;

    private void Start()
    {
        // initially hide the prompt text
        ammoPromptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ammoPromptText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // hides prompt
            ammoPromptText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetButtonDown("Interact"))
            {
                PlayerController player = other.GetComponent<PlayerController>();

                if (player != null)
                {
                    PurchaseAmmo(player);
                }
            }
        }
    }

    public void PurchaseAmmo(PlayerController player)
    {
        if (gameManager.instance.GetPlayerMoney() >= ammoCost)
        {
            gameManager.instance.SpendMoney(ammoCost);

            weaponHandler equippedHandler = player.GetWeaponEquipped();  
            if (equippedHandler != null)
            {
                // resupply ammo
                equippedHandler.Resupply(ammoAmount); 
            }
            else
            {
                Debug.LogWarning("WeaponHandler not found.");
            }
        }
    }
}
