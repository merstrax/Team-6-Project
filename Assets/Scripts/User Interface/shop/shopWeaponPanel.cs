using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class shopWeaponPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] TextMeshProUGUI weaponCost;

    public void SetWeaponName(string name)
    {
        weaponName.text = name;
    }

    public void SetWeaponCost(int amount)
    {
        weaponCost.text = amount.ToString();
    }
}
