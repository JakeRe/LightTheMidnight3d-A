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
    [SerializeField] private GameObject flashlightHitBox;
    [Tooltip("The light gameobject used to cast the Flashlight beam")]
    [SerializeField] public Light flashLightEmitter;
    [Tooltip("The controller of the Player")]
    [SerializeField] private PlayerController player;

    [Header("Light Management")]
    [Tooltip("Tells if the flashlight is ready for use")]
    [SerializeField] public bool isReady;
    [Tooltip("Indicates if the flashlight is toggled on.")]
    [SerializeField] public bool isOn = false;
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


    [Header("Sound Materials")]
    [SerializeField] private AudioSource weaponSoundSource;
    [SerializeField] private AudioClip[] flashlightSounds;
  


    //Spotlight properties
    [SerializeField] private bool isCharging;
    [SerializeField] private bool isFiring;
    [SerializeField] private float chargeStartedTime;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float shotStartedTime;
    [SerializeField] private float shotDuration;
    [SerializeField] private float currentTime;


    private void OnEnable()
    {
        PlayerUI.batteryUpdate += ActiveWeapon;

    }

    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        thisWeaponPv = GetComponent<PhotonView>();
        weaponSoundSource = GetComponent<AudioSource>();
        playerUI = FindObjectOfType<PlayerUI>();
        if (weaponID == 1)
        {
            isOn = false;
            flashLightEmitter.gameObject.SetActive(false);
            flashlightHitBox.gameObject.SetActive(false);
        }
    }

    void Update()
    { 
        if (player.photonView.IsMine)
        {
            //this.BatteryManagement();
            //this.Attack();
            this.WeightCheck();
            this.ToggleFlashlight();
            if (weaponID == 0)
                this.FlashlightManagement();
            if (weaponID == 1)
                if (isOn && isReady)
                    StartCoroutine("SpotLightShot");
            this.BatteryUpdate();
            
            
        }

        currentTime = Time.time;
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
        
        if(batteryLevel <= 0)
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
                weaponSoundSource.PlayOneShot(flashlightSounds[0]);
                isOn = true;
            }
            else if (isOn && !isCharging && !isFiring)
            {
                weaponSoundSource.PlayOneShot(flashlightSounds[1]);
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
            if (canRecharge)
            {
                if (batteryLevel <= batteryLevelMax)
                    batteryLevel += batteryRecharge * Time.deltaTime;
                if (flashLightEmitter.range <= maxFlashlightRange)
                    flashLightEmitter.range += batteryRecharge / 2 * Time.deltaTime;
            }
        }
    }

    void SpotLightAttack()
    {
        if (isOn && !isCharging)
        {
            isCharging = true;
            chargeStartedTime = Time.time;
            player.canMove = false;

            if (Time.time - chargeStartedTime >= chargeDuration)
            {
                player.canRotate = false;
                shotStartedTime = Time.time;
                isFiring = true;
                flashLightEmitter.gameObject.SetActive(true);
                flashlightHitBox.gameObject.SetActive(true);
                
                if (Time.time - shotStartedTime >= shotDuration)
                {
                    isOn = false;
                    isCharging = false;
                    isFiring = false;
                }
            }
        }
        else if (!isOn)
        {
            flashLightEmitter.gameObject.SetActive(false);
            flashlightHitBox.gameObject.SetActive(false);
        }
    }

    IEnumerator SpotLightShot()
    {
        if (!isCharging && !isFiring)
        {
            isReady = false;
            isCharging = true;
            player.canMove = false;
            yield return new WaitForSeconds(chargeDuration);
            flashLightEmitter.gameObject.SetActive(true);
            flashlightHitBox.gameObject.SetActive(true);
            isCharging = false;
            isFiring = true;
            player.canRotate = false;
            Cursor.lockState = CursorLockMode.Locked;
            yield return new WaitForSeconds(shotDuration);
            flashLightEmitter.gameObject.SetActive(false);
            flashlightHitBox.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            isFiring = false;
            isOn = false;
            isReady = true;
            player.canRotate = true;
            player.canMove = true;
            yield break;
        }
    }
}

