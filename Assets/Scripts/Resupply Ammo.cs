using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResupplyAmmo : MonoBehaviour
{
    [Range(0, 500)][SerializeField] int ammoCost;
    [Range(10, 100)][SerializeField] int ammoAmount;

    private PlayerController currentPlayer; 

    private void Start()
    {
        // initially hide the prompt text
        //ammoPromptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.ShowInteractMessage();
            currentPlayer = other.GetComponent<PlayerController>(); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // hides prompt
            gameManager.instance.HideInteractMessage();
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
                PurchaseAmmo(currentPlayer);
            }
        }
    }
    private void ShowResupplyMenu()
    {
        if (currentPlayer != null)
        {
            // Show the resupply menu UI
            //resupplyMenuUI.SetActive(true);

            // Unlock and show the cursor
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;

            // Pause the game
            Time.timeScale = 0f; 
        }
    }
   
    private void CloseResupplyMenu()
    {
        //resupplyMenuUI.SetActive(false);

        // Lock and hide the cursor again
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;

        // Resume the game
        Time.timeScale = 1f; 
    }
    public void BuyAmmo() 
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
                equippedHandler.UpdateUI();
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
