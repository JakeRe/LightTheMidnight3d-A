using System.Collections;
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

    [SerializeField] private string weaponName;
    [SerializeField] public int weaponID;

    [Header("Flashlight")]
    [Tooltip("Hitbox for the flashlight")]
    [SerializeField] protected GameObject flashlightHitBox;
    [Tooltip("The light gameobject used to cast the Flashlight beam")]
    [SerializeField] public Light flashLightEmitter;
    [Tooltip("The controller of the Player")]
    [SerializeField] protected PlayerController player;

    [Header("Light Management")]
    [Tooltip("Tells if the flashlight is ready for use")]
    [SerializeField] public bool isReady;
    [Tooltip("Indicates if the flashlight is toggled on.")]
    [SerializeField] public bool isOn = false;
    [Tooltip("Color emitted by flashlight")]
    [SerializeField] private Color lightColor;
    [Tooltip("Flashlight maximum range")]
    [SerializeField] public float maxFlashlightRange;


    [Header("Weapon Specifics")]
    [SerializeField] protected PlayerUI playerUI;
    [Tooltip("Determines if the weapon can recharge")]
    [SerializeField] protected bool canRecharge;
    [Tooltip("Current Battery Level of Flashlight")]
    [SerializeField] public float batteryLevel;
    [Tooltip("Maximum Battery Life for Flashlight")]
    [SerializeField] public float batteryLevelMax;
    [Tooltip("Rate by which the battery for the flashlight drains")]
    [SerializeField] protected float batteryDrain;
    [Tooltip("Rate by which the battery for the flashlight recharges")]
    [SerializeField] protected float batteryRecharge;
    [Tooltip("The Weight of the Equipped Weapon")]
    [SerializeField] protected float weight;
    [Tooltip("This weapon's damage value")]
    [SerializeField] public float damageRate;

    [SerializeField] private float moveSpeed;


    [Header("Sound Materials")]
    [SerializeField] protected AudioSource weaponSoundSource;
    [SerializeField] protected AudioClip[] flashlightSounds;
    [SerializeField] protected AudioClip[] spotlightSounds;

    [SerializeField] protected bool isCharging;
    [SerializeField] protected bool isFiring;
    [SerializeField] protected GameManager gameManager;

    void Awake()
    {

        player = GetComponentInParent<PlayerController>();
        thisWeaponPv = GetComponent<PhotonView>();
        weaponSoundSource = GetComponent<AudioSource>();
        playerUI = FindObjectOfType<PlayerUI>();
        gameManager = FindObjectOfType<GameManager>();
        if (weaponID == 1)
        {
            isOn = false;
            flashLightEmitter.gameObject.SetActive(false);
            flashlightHitBox.gameObject.SetActive(false);
        }
    }

    void Update()
    { 
        if (player.photonView.IsMine && !gameManager.isPaused)
        {
          
            this.WeightCheck();
            this.ToggleFlashlight();
            if (weaponID != 1)
                this.FlashlightManagement();
            this.BatteryUpdate();
            
            
        }

       
    }
    public void BatteryUpdate()
    {
        playerUI.weaponBattery.maxValue = batteryLevelMax;
        playerUI.weaponBattery.value = batteryLevel;
        
        if(batteryLevel <= 0)
        {
            playerUI.batteryLevel.sprite = playerUI.emptyBattery;
        }
    }

    void WeightCheck()
    {

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

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(batteryLevel);
    //        stream.SendNext(lightColor);
    //        stream.SendNext(gameObject.activeSelf);
    //    }
    //    else if (stream.IsReading)
    //    {
    //        this.batteryLevel = (float)stream.ReceiveNext();
    //        this.lightColor = (Color)stream.ReceiveNext();
    //        this.gameObject.SetActive((bool)stream.ReceiveNext());
    //    }
    //}

    /// <summary>
    /// This method will toggle the flashlight on or off when the left mouse button is clicked.
    /// When checking if it can toggle on, the method will also see if the flashlight is ready
    /// (if it's not in a cooldown state) otherwise it won't turn on.
    /// </summary>
    protected void ToggleFlashlight()
    {
        if (Input.GetButtonDown("Fire1"))
        {
                if (!isOn)
                {
                  isOn = true;
                if(weaponID != 1) {
                    weaponSoundSource.PlayOneShot(flashlightSounds[0]);
                }
                 
                   
                }
                else if (isOn && !isCharging && !isFiring)
                {
                  isOn = false;
                if(weaponID != 1)
                {
                    weaponSoundSource.PlayOneShot(flashlightSounds[1]);
                }   
                }
        }
    }

    void FlashlightManagement()
    {
        if (isOn && playerUI != null)
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
        else if (!isOn && playerUI != null)
        {
            flashLightEmitter.gameObject.SetActive(false);
            flashlightHitBox.gameObject.SetActive(false);
            playerUI.weaponBattery.maxValue = batteryLevelMax;
            playerUI.weaponBattery.value = batteryLevel;
            if (canRecharge)
            {
                if (batteryLevel <= batteryLevelMax)
                    batteryLevel += batteryRecharge * Time.deltaTime;
                if (flashLightEmitter.range <= maxFlashlightRange)
                    flashLightEmitter.range += batteryRecharge / 2 * Time.deltaTime;
            }
        }
    }

}

