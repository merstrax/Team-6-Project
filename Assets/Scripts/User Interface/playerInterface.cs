using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerInterface : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textWave;
    [SerializeField] TextMeshProUGUI textEnemiesRemaining;
    [SerializeField] TextMeshProUGUI textEnemiesKilled;
    [SerializeField] TextMeshProUGUI textMoney;
    [SerializeField] TextMeshProUGUI textAmmoCurrent;
    [SerializeField] TextMeshProUGUI textAmmoMax;
    [SerializeField] Image playerHealthBar;

    const string WAVE_TEXT = "Wave: ";
    const string REMAIN_TEXT = "Remaining: ";
    const string KILLED_TEXT = "Killed: ";
    const string MONEY_TEXT = "$";

    void Start()
    {
        UpdatePlayerInterface();
    }

    public void UpdatePlayerInterface()
    {
        textWave.text = WAVE_TEXT + gameManager.instance.GetCurrentWave().ToString();
        textEnemiesRemaining.text = REMAIN_TEXT + gameManager.instance.GetEnemiesRemaining().ToString();
        textEnemiesKilled.text = KILLED_TEXT + gameManager.instance.GetEnemiesKilled().ToString();
        textMoney.text = MONEY_TEXT + gameManager.instance.GetPlayerMoney().ToString();
    }

    public void UpdatePlayerAmmo(string currentAmmo, string maxAmmo)
    {
        textAmmoCurrent.text = currentAmmo;
        textAmmoMax.text = "/" + maxAmmo;
    }
    public void UpdatePlayerHealth(int health, int maxHealth)
    {
        playerHealthBar.fillAmount = (float)health / maxHealth;
    }
}
