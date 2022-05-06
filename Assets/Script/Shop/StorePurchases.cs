using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StorePurchases : ShopManager
{
    // Start is called before the first frame update

    [Header("The Price of Each Particular Upgrade")]
    [SerializeField] private float spotLightRefill;
    [SerializeField] private float healthUpgrade;
    [SerializeField] private float healthRefill;
    [SerializeField] private float laserSwordCost;
    [SerializeField] private float damageUpgradeCost;

    [Header("Upgrade for Damages")]
    [SerializeField] private float damageUpgrade;

    [Header("Damage Upgrade Text")]
    [SerializeField] private string costOfUpgrade;
    [SerializeField] private TextMeshProUGUI upgradeCostText;


    [Header("Manages weapon spawn locations")]
    [SerializeField] private Vector3 weaponSpawnOffset;
    [SerializeField] private WeaponManagement weaponManage;
    [SerializeField] private GameObject weaponSpawn;
    [Header("Weapons Purchasable in the Shop")]
    [SerializeField] private GameObject[] weapons;
    [Header("Weapons Stored for Upgrades")]
    [SerializeField] private SpotlightWeapon weapon;

    [Header("Ugrade Buttons")]
    [SerializeField] private List<Button> buttons;


    private new void Start()
    {
        upgradeCostText.text = $"{damageUpgradeCost} Plasm";
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
        
        cost = spotLightRefill;

        if(weapon.shotCount < weapon.maxShotCount)
        {
            buttons[0].interactable = true;
        }
        if (weapon != null && weapon.shotCount != weapon.maxShotCount && points >= cost)
        {
            weapon.shotCount += 1;
            weapon.BatteryUpdate();
            Purchase();
            Debug.Log($"Refilled weapon to {weapon.shotCount}");
        }
        else if (weapon.shotCount >= weapon.maxShotCount)
        {
            buttons[0].interactable = false;
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
        else if(player.maxHealth >= player.maxHealthAbsolute)
        {
            buttons[1].interactable = false;
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
           Weapons laser = melee.GetComponent<Weapons>();
           laser.weaponID = weaponManage.weaponObjects.Count;
           laser.weaponID -= 1;
           
        }
    }

    public void DamageUpgrade()
    {
        cost = damageUpgradeCost;

        if (player != null && player.playerPoints >= cost) { 
        Weapons standardWeapon = FindObjectOfType<Weapons>();
        standardWeapon.damageRate += damageUpgrade;
        damageUpgradeCost += damageUpgradeCost;
        Purchase();

        }

        upgradeCostText.text = $"{damageUpgradeCost} Plasm";


    }
}
