using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class shopManager : MonoBehaviour
{
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject weaponPanel;
    [SerializeField] GameObject[] weapons;
    [SerializeField] GameObject[] weaponsShop;

    //Layout variables for each of the weapon panels
    [SerializeField] int startX = -630;
    [SerializeField] int startY = 210;

    [SerializeField] int width = 340;
    [SerializeField] int height = 200;

    [SerializeField] int col = 4;
    [SerializeField] int padding = 30;

    void Start()
    {
        //Temp variables for creating the shop panels
        GameObject tempPanel;
        GameObject tempWeapon;
        shopWeaponPanel panelInfo;
        weaponHandler weaponInfo;

        int offsetX;
        int offsetY;

        for(int i = 0; i < weapons.Length; i++)
        {
            offsetX = startX + ((width + padding) * (i % col));
            offsetY = startY - ((height + padding) * (i / col));

            //Create a new weaponPanel
            tempPanel = Instantiate(weaponPanel);
            tempPanel.transform.SetParent(shopPanel.transform, true);
            tempPanel.transform.localPosition = new Vector3(offsetX, offsetY, shopPanel.transform.position.z);
            tempPanel.transform.localScale = Vector3.one;

            panelInfo = tempPanel.GetComponent<shopWeaponPanel>();

            if (panelInfo != null)
            {
                //Create a new shopWeapon
                tempWeapon = Instantiate(weaponsShop[i]);
                tempWeapon.transform.SetParent(tempPanel.transform, true);
                tempWeapon.transform.localPosition = weaponsShop[i].transform.localPosition;
                tempWeapon.transform.localScale = weaponsShop[i].transform.localScale;

                //Set the elements values
                tempWeapon = Instantiate(weapons[i]);
                weaponInfo = tempWeapon.GetComponent<weaponHandler>();
                if (weaponInfo != null)
                {
                    panelInfo.SetWeaponName(weaponInfo.GetWeaponName());
                    panelInfo.SetWeaponCost(weaponInfo.GetWeaponCost());
                }
            }
        }

    }
}
