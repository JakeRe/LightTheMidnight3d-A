using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFFRefill : ShopManager
{  
    void SpotCannonRefill()
    {
        SpotlightWeapon weapon = FindObjectOfType<SpotlightWeapon>();

        if (weapon != null && weapon.shotCount != weapon.maxShotCount)
        {
            weapon.shotCount += 1;
            points -= cost;
        }
    }

}
