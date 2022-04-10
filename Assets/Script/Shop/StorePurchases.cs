using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePurchases : ShopManager
{
    // Start is called before the first frame update

    [Header("The Price of Each Particular Upgrade")]
    [SerializeField] private float spotLightRefill;
    [SerializeField] private float healthUpgrade;
    [SerializeField] private float healthRefill;
    [SerializeField] private float laserSwordCost;

    [SerializeField] private Vector3 weaponSpawnOffset;
    [SerializeField] private WeaponManagement weaponManage;
    [SerializeField] private GameObject weaponSpawn;

    [SerializeField] private GameObject[] weapons;


    private new void Start()
    {
        base.Start();
        weaponSpawn = GameObject.FindGameObjectWithTag("WeaponSpawn");
        weaponManage = GameObject.FindObjectOfType<WeaponManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        StatsUpdated();
    }

    void Purchase()
    {
        player.playerPoints -= cost;
        playerPoints.text = points.ToString();
    }

    public void SpotCannonRefill()
    {
        SpotlightWeapon weapon = (SpotlightWeapon)FindObjectOfType(typeof(SpotlightWeapon));

        cost = spotLightRefill;

        if (weapon != null && weapon.shotCount != weapon.maxShotCount && points >= cost)
        {
            weapon.shotCount += 1;
            Purchase();
            Debug.Log($"Refilled weapon to {weapon.shotCount}");
        }
        else
        {
            Debug.Log("Purchase Failed");
            Debug.Log($"Points Left {points}");
        }
    }

    public void HealthUpgrade()
    {
        cost = healthUpgrade;

        if(player != null && points >= cost && player.maxHealth < player.maxHealthAbsolute)
        {
            player.maxHealth += 1;
            player.health = player.maxHealth;
            Purchase();
        }

      
    }

    public void HealthRefill()
    {
        cost = healthRefill;

        if(player != null && points >= cost)
        {
            player.health += 1;
            Purchase();
        }
    }

    public void LaserSwordPurchase()
    {
        cost = laserSwordCost;

        if(player != null && points >= cost)
        {
           player.playerPoints -= cost;
           playerPoints.text = points.ToString();
           GameObject melee =  Instantiate(weapons[0], weaponSpawn.transform);
           melee.transform.SetParent(weaponManage.transform);
           melee.SetActive(false);
           weaponManage.weaponObjects.Add(melee);
        }
    }

}
