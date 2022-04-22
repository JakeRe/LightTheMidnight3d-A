using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using VLB;

public class Weapons : MonoBehaviour
{
    [Header("Photon View")]
    [SerializeField] private PhotonView thisWeaponPv;

    [SerializeField] private string weaponName;
    [SerializeField] public int weaponID;

    [Header("Flashlight")]
    [Tooltip("Hitbox for the flashlight")]
    [SerializeField] protected GameObject flashlightHitBox;
    [Tooltip("The controller of the Player")]
    [SerializeField] protected PlayerController player;
    [Tooltip("Volumetric Light Beam")]
    [SerializeField] protected VolumetricLightBeam flashlightBeam;

    [Header("Light Management")]
    [Tooltip("Tells if the flashlight is ready for use")]
    [SerializeField] public bool isReady;
    [Tooltip("Indicates if the flashlight is toggled on.")]
    [SerializeField] public bool isOn = false;
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
    [SerializeField] protected float batteryRecharge;
    [Tooltip("This weapon's damage value")]
    [SerializeField] public float damageRate;
    public float defaultDamage;

    [SerializeField] protected bool canFire;

    [Header("Sound Materials")]
    [SerializeField] protected AudioSource weaponSoundSource;
    [SerializeField] protected AudioClip[] flashlightSounds;
    [SerializeField] protected AudioClip[] spotlightSounds;

    [SerializeField] protected bool isCharging;
    [SerializeField] protected bool isFiring;
    [SerializeField] protected GameManager gameManager;

    [Header("UI for Weapons")]
    [SerializeField] protected Slider weaponCharge;

    //private void OnEnable()
    //{
    //    WeaponManagement.OnActive += CanFire;
    //}

    //private void OnDisable()
    //{
    //    WeaponManagement.OnActive -= CanFire;
    //}

    void Awake()
    {
        defaultDamage = damageRate;
        flashlightBeam = GetComponentInChildren<VolumetricLightBeam>();
        player = GetComponentInParent<PlayerController>();
        thisWeaponPv = GetComponent<PhotonView>();
        weaponSoundSource = GetComponent<AudioSource>();
        playerUI = FindObjectOfType<PlayerUI>();
        gameManager = FindObjectOfType<GameManager>();
        weaponCharge = GetComponentInChildren<Slider>();

        if (flashlightBeam != null)
        {
            flashlightBeam.enabled = false;
        }

        if (weaponID == 1)
        {
            isOn = false;
            flashlightHitBox.gameObject.SetActive(false);
            
           
        }
    }

    void Update()
    { 
        if (player.photonView.IsMine && !gameManager.isPaused && !player.inShop)
        {
          
            this.ToggleFlashlight();
            this.FlashlightManagement();
            this.BatteryUpdate();
            
            
        }

       
    }
    public virtual void BatteryUpdate()
    {

        if (this.gameObject.activeSelf && weaponCharge != null)
        {
            weaponCharge.gameObject.SetActive(true);
        }
        else
        {
            weaponCharge.gameObject.SetActive(false);
        }

        weaponCharge.maxValue = batteryLevelMax;
        weaponCharge.value = batteryLevel;
        
        if(batteryLevel <= 0)
        {
            playerUI.batteryLevel.sprite = playerUI.emptyBattery;
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

    

    /// <summary>
    /// This method will toggle the flashlight on or off when the left mouse button is clicked.
    /// When checking if it can toggle on, the method will also see if the flashlight is ready
    /// (if it's not in a cooldown state) otherwise it won't turn on.
    /// </summary>
    public virtual void ToggleFlashlight()
    {
        CanFire();
        if (Input.GetButtonDown("Fire1") && canFire)
        {
                if (!isOn)
                {
                  isOn = true;
              
                    weaponSoundSource.PlayOneShot(flashlightSounds[0]);
                }
                else if (isOn && !isCharging && !isFiring)
                {
                  isOn = false;
               
                    weaponSoundSource.PlayOneShot(flashlightSounds[1]);
                
                }
        }
    }

    void FlashlightManagement()
    {
        if (isOn && playerUI != null)
        {
            if(flashlightBeam != null)
            {
                flashlightBeam.enabled = true;
            }
            playerUI.weaponBattery.maxValue = batteryLevelMax;
            
            if (batteryLevel >= 0)
                batteryLevel = batteryLevel -= batteryDrain * Time.deltaTime;
            flashlightHitBox.gameObject.SetActive(true);
          
            playerUI.weaponBattery.value = batteryLevel;
        }
        else if (!isOn && playerUI != null)
        {


            if (flashlightBeam != null)
            {
                flashlightBeam.enabled = false;
            }

            flashlightHitBox.gameObject.SetActive(false);
            playerUI.weaponBattery.maxValue = batteryLevelMax;
            playerUI.weaponBattery.value = batteryLevel;
            if (canRecharge)
            {
                if (batteryLevel <= batteryLevelMax)
                    batteryLevel += batteryRecharge * Time.deltaTime;
               
            }
        }
    }


    void CanFire()
    {
        if (this.gameObject.activeInHierarchy)
        {
            canFire = true;
        }
        else
        {
            canFire = false;
        }

       
    }
}

