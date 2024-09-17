using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class shopWeaponPanel : MonoBehaviour
{
    [SerializeField] weaponHandler weapon;
    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] TextMeshProUGUI weaponCost;

    private void Start()
    {
        SetWeaponName(weapon.GetWeaponName());
        SetWeaponCost(weapon.GetWeaponCost());
    }

    public void SetWeaponName(string name)
    {
        weaponName.text = name;
    }

    public void SetWeaponCost(int amount)
    {
        weaponCost.text = amount.ToString();
    }

    public int GetWeaponCost()
    {
        return weapon.GetWeaponCost();
    }

    public weaponHandler GetWeaponHandler()
    {
        return weapon;
    }
}
