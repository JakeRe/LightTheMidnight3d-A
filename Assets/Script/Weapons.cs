﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class Weapons : MonoBehaviour
{
    [Header("Photon View")]
    [SerializeField] private PhotonView thisWeaponPv;

    [Header("Flashlight")]
    [Tooltip("Hitbox for the flashlight")]
    [SerializeField] private GameObject flashlightHitBox;
    [Tooltip("The light gameobject used to cast the Flashlight beam")]
    [SerializeField] public Light flashLightEmitter;
    [Tooltip("The controller of the Player")]
    [SerializeField] private PlayerController player;

    [Header("Light Management")]
    [Tooltip("Tells if the flashlight is ready for use")]
    [SerializeField] public bool isReady;
    [Tooltip("Indicates if the flashlight is toggled on.")]
    [SerializeField] public bool isOn = true;
    [Tooltip("Color emitted by flashlight")]
    [SerializeField] private Color lightColor;
    [Tooltip("Flashlight maximum range")]
    [SerializeField] public float maxFlashlightRange;
    //[Tooltip("If the flashlight is in Use")]
    //[SerializeField] public bool isActive;

    [Header("Weapon Specifics")]
    [SerializeField] private PlayerUI playerUI;
    [Tooltip("Determines if the weapon can recharge")]
    [SerializeField] private bool canRecharge;
    [Tooltip("Current Battery Level of Flashlight")]
    [SerializeField] public float batteryLevel;
    [Tooltip("Maximum Battery Life for Flashlight")]
    [SerializeField] public float batteryLevelMax;
    [Tooltip("Rate by which the battery for the flashlight drains")]
    [SerializeField] private float batteryDrain;
    [Tooltip("Rate by which the battery for the flashlight recharges")]
    [SerializeField] private float batteryRecharge;
    [Tooltip("The Weight of the Equipped Weapon")]
    [SerializeField] private float weight;
    [Tooltip("This weapon's damage value")]
    [SerializeField] public float damageRate;

    [SerializeField] private float moveSpeed;
  

    private void OnEnable()
    {
        PlayerUI.batteryUpdate += ActiveWeapon;

    }

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        thisWeaponPv = GetComponent<PhotonView>();
        playerUI = FindObjectOfType<PlayerUI>();
    }

    void Update()
    { 
        if (player.photonView.IsMine)
        {
            //this.BatteryManagement();
            //this.Attack();
            this.WeightCheck();
            this.ToggleFlashlight();
            this.FlashlightManagement();
            this.BatteryUpdate();
            
            
        }
    }
    void BatteryManagement()
    {
        if (this.batteryLevel <= 0)
        {
            this.isReady = false;
            this.flashlightHitBox.gameObject.SetActive(false);
            this.flashLightEmitter.gameObject.SetActive(false);
        }

        if (batteryLevel <= batteryLevelMax && !isReady && canRecharge)
        {
            StartCoroutine("FlashLightCoolDown");
        }
    }

   /* void Attack()
    {
        if (Input.GetButton("Fire1") && batteryLevel >= 0 && isReady)
        {
            playerUI.weaponBattery.maxValue = batteryLevelMax;
            //isActive = true;
            flashLightEmitter.gameObject.SetActive(true);
            batteryLevel = batteryLevel -= batteryDrain * Time.deltaTime;
            flashlightHitBox.gameObject.SetActive(true);
            flashLightEmitter.range -= flashLightEmitter.range * Time.deltaTime;
            playerUI.weaponBattery.value = batteryLevel;

        }

    }*/

    public void BatteryUpdate()
    {
        playerUI.weaponBattery.maxValue = batteryLevelMax;
        playerUI.weaponBattery.value = batteryLevel;
        
        if(batteryLevel == 0)
        {
            playerUI.batteryLevel.sprite = playerUI.emptyBattery;
        }
    }

    void WeightCheck()
    {
        /*
         * This code can probably be removed. I changed it so the player's movement speed
         * is automatically set to the speed provided by the weapon. Easier to change in
         * the inspector and avoids having hard-coded values.
        if(gameObject.tag == "Heavy")
        {
            player.movementSpeed = 4;
        }
        else
        {
            player.movementSpeed = 12;
        }
        */

        player.movementSpeed = moveSpeed;
    }

    IEnumerator FlashLightCoolDown()
    {
        //Light waits for five seconds before recharging.
        yield return new WaitForSeconds(5);
        //If the battery is not ready and less than 100
        if (batteryLevel < 100 && !isReady)
        {
            //Battery is added to over the course of time
            batteryLevel += batteryDrain * Time.deltaTime;
            flashLightEmitter.color += (lightColor) * Time.deltaTime;
            flashLightEmitter.range = maxFlashlightRange;

            //If the battery is full, then break the loop
            if (batteryLevel >= batteryLevelMax)
            {
                batteryLevel = batteryLevelMax;
                playerUI.batteryLevel.sprite = playerUI.fullBattery;
                isReady = true;
                yield break;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(batteryLevel);
            stream.SendNext(lightColor);
            stream.SendNext(gameObject.activeSelf);
        }
        else if (stream.IsReading)
        {
            this.batteryLevel = (float)stream.ReceiveNext();
            this.lightColor = (Color)stream.ReceiveNext();
            this.gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }

    public void ActiveWeapon()
    {
        
    }

    /*
     * if (isOn)
     *      light is active
     *      hitbox is active
     *      drain battery
     * else if (!isOn)
     *      light is inactive
     *      hitbox is inactive
     *      battery recharges
    */

    /// <summary>
    /// This method will toggle the flashlight on or off when the left mouse button is clicked.
    /// When checking if it can toggle on, the method will also see if the flashlight is ready
    /// (if it's not in a cooldown state) otherwise it won't turn on.
    /// </summary>
    void ToggleFlashlight()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isOn)
            {
                isOn = true;
            }
            else if (isOn)
            {
                isOn = false;
            }
        }
    }

    void FlashlightManagement()
    {
        if (isOn)
        {
            playerUI.weaponBattery.maxValue = batteryLevelMax;
            flashLightEmitter.gameObject.SetActive(true);
            if (batteryLevel >= 0)
                batteryLevel = batteryLevel -= batteryDrain * Time.deltaTime;
            flashlightHitBox.gameObject.SetActive(true);
            if (flashLightEmitter.range >= 0)
                flashLightEmitter.range -= batteryDrain/3.5f * Time.deltaTime;
            playerUI.weaponBattery.value = batteryLevel;
        }
        else if (!isOn)
        {
            flashLightEmitter.gameObject.SetActive(false);
            flashlightHitBox.gameObject.SetActive(false);
            playerUI.weaponBattery.maxValue = batteryLevelMax;
            playerUI.weaponBattery.value = batteryLevel;
            if (batteryLevel <= batteryLevelMax)
                batteryLevel += batteryRecharge * Time.deltaTime;
            if (flashLightEmitter.range <= maxFlashlightRange)
                flashLightEmitter.range += batteryRecharge/2 * Time.deltaTime; //revert to flashLightEmitter.range instsead of batteryRecharge if needed
            //flashLightEmitter.color += (lightColor) * Time.deltaTime;

        }
    }
}

