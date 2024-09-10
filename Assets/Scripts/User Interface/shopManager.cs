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

    //Layout variables for each of the weapon panels
    private int startX = -630;
    private int startY = 210;
    
    private int width = 340;
    private int height = 200;

    private int col = 4;
    private int padding = 30;

    void Start()
    {
        //Temp variables for creating the shop panels
        GameObject tempPanel;
        GameObject tempWeapon;
        shopWeaponPanel panelInfo;
        shopWeaponInfo weaponInfo;

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
                tempWeapon = Instantiate(weapons[i]);
                tempWeapon.transform.SetParent(tempPanel.transform, true);
                tempWeapon.transform.localPosition = weapons[i].transform.localPosition;
                tempWeapon.transform.localScale = weapons[i].transform.localScale;

                //Set the elements values
                weaponInfo = tempWeapon.GetComponent<shopWeaponInfo>();
                if (weaponInfo != null)
                {
                    panelInfo.SetWeaponName(weaponInfo.GetWeaponName());
                    panelInfo.SetWeaponCost(weaponInfo.GetWeaponCost());
                }
            }
        }

    }
}
