using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResupplyAmmo : MonoBehaviour
{
    [Range(0, 500)][SerializeField] int ammoCost;
    [Range(10, 100)][SerializeField] int ammoAmount;
    [SerializeField] TextMeshProUGUI ammoPromptText;
    [SerializeField] GameObject resupplyMenuUI; 

    private PlayerController currentPlayer; 

    private void Start()
    {
        // initially hide the prompt text
        ammoPromptText.gameObject.SetActive(false);
        resupplyMenuUI.SetActive(false); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ammoPromptText.gameObject.SetActive(true);
            currentPlayer = other.GetComponent<PlayerController>(); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // hides prompt
            ammoPromptText.gameObject.SetActive(false);
            resupplyMenuUI.SetActive(false);
            currentPlayer = null; 
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check for interaction input while the player is in the trigger
            if (Input.GetButtonDown("Interact"))
            {
                // Display the resupply menu when the player presses the interact button
                ShowResupplyMenu();
            }
        }
    }
    private void ShowResupplyMenu()
    {
        if (currentPlayer != null)
        {
            // Show the resupply menu UI
            resupplyMenuUI.SetActive(true);
        }
    }
   
    private void CloseResupplyMenu()
    {
        resupplyMenuUI.SetActive(false);
    }
    private void BuyAmmo()
    {
        if (currentPlayer != null)
        {
            // Purchase ammo directly
            PurchaseAmmo(currentPlayer);

            // Close the resupply menu after buying ammo
            CloseResupplyMenu();
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
        else
        {
            Debug.LogWarning("Not enough money."); 
        }
    }
}
