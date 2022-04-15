using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LanternWeapon : Weapons
{
    [SerializeField] private NavMeshObstacle radius;
    [SerializeField] private bool isDeployed;
    [SerializeField] private BoxCollider pickupRadius;
    [SerializeField] private WeaponManagement weaponManage;
    [SerializeField] private GameObject weaponSpawn;
    [SerializeField] private Rigidbody rb;
   
    void OnEnable()
    {
         PlayerController.PickUp += PickUp;
    }

    private void OnDisable()
    {
        PlayerController.PickUp -= PickUp;
    }

    private void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
        radius = GetComponent<NavMeshObstacle>();
        weaponSpawn = GameObject.FindGameObjectWithTag("WeaponSpawn");
        weaponManage = GameObject.FindObjectOfType<WeaponManagement>();
        rb = GetComponent<Rigidbody>();
    }

     void Update()
    {
        BatteryUpdate();
        Drop();
    }

    void PickUp()
    {
        playerUI.DisableInteract();
        this.transform.position = weaponSpawn.transform.position;
        isDeployed = false;
        rb.isKinematic = true;
        radius.enabled = false;
        this.transform.SetParent(weaponManage.transform);
        gameObject.SetActive(false);
        weaponManage.weaponObjects.Add(this.gameObject);
        this.weaponID = weaponManage.weaponObjects.Count;
        this.weaponID -= 1;
    }

    void Drop()
    {
        if (Input.GetKeyDown(KeyCode.E) && gameObject.activeSelf)
        {
            rb.isKinematic = false;
            radius.enabled = true;
            this.transform.SetParent(null);
            isDeployed = true;
            weaponManage.weaponObjects.Remove(this.gameObject);
        }
    }

    public override void BatteryUpdate()
    { 
        if(isDeployed && batteryLevel <= batteryLevelMax)
        {
            batteryLevel -= batteryDrain * Time.deltaTime;
        }

        if(batteryLevel <= 0)
        {
            radius.radius = 0;
        }
    }
}
