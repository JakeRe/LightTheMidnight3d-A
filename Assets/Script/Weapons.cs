using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Weapons : MonoBehaviour
{
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
    [Tooltip("Color emitted by flashlight")]
    [SerializeField] private Color lightColor;
    [Tooltip("Flashlight maximum range")]
    [SerializeField] public float maxFlashlightRange;
    [Tooltip("If the flashlight is in Use")]
    [SerializeField] public bool isActive;

    [Header("Weapon Specifics")]
    [Tooltip("Determines if the weapon can recharge")]
    [SerializeField] private bool canRecharge;
    [Tooltip("Current Battery Level of Flashlight")]
    [SerializeField] public float batteryLevel;
    [Tooltip("Maximum Battery Life for Flashlight")]
    [SerializeField] public float batteryLevelMax;
    [Tooltip("Rate by which the battery for the flashlight drains")]
    [SerializeField] private float batteryDrain;
    [Tooltip("The Weight of the Equipped Weapon")]
    [SerializeField] private float weight;

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    void Update()
    { 
        if (player.photonView.IsMine)
        {
            this.BatteryManagement();
            this.Attack();
            this.WeightCheck();
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

    void Attack()
    {

        if (Input.GetButton("Fire1") && batteryLevel >= 0 && isReady)
        {
            isActive = true;
            flashLightEmitter.gameObject.SetActive(true);
            batteryLevel = batteryLevel -= batteryDrain * Time.deltaTime;
            flashlightHitBox.gameObject.SetActive(true);
            flashLightEmitter.range -= flashLightEmitter.range * Time.deltaTime;

        }

    }

    void WeightCheck()
    {
        if(gameObject.tag == "Heavy")
        {
            player.movementSpeed = 4;
        }
        else
        {
            player.movementSpeed = 12;
        }
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
                isReady = true;
                yield break;
            }
        }
    }

    
}
