using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopWeaponInfo : MonoBehaviour
{
    [SerializeField] string weaponName;
    [SerializeField] int weaponCost;

    public string GetWeaponName()
    {
        return weaponName;
    }

    public int GetWeaponCost()
    {
        return weaponCost;
    }
}
