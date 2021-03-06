using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpotlightWeapon : Weapons

{
    #region Spotlight Properties
    //Spotlight properties
    [Tooltip("When the charge starts to activate")]
    [SerializeField] protected float chargeStartedTime;
    [Tooltip("How long the charge takes to build up")]
    [SerializeField] protected float chargeDuration;
    [Tooltip("The time when the weapon fires")]
    [SerializeField] protected float shotStartedTime;
    [Tooltip("How long this weapon's hit box is active")]
    [SerializeField] protected float shotDuration;
    [Tooltip("What the current time in relation to when the weapon was fired")]
    [SerializeField] protected float currentTime;
    [Tooltip("How many shots this weapon has")]
    [SerializeField] public float shotCount;
    [Tooltip("The maximum amount of shots this weapon can hold")]
    [SerializeField] public float maxShotCount;
    [SerializeField] public Image[] shotIcons;
    [SerializeField] private Sprite fullCharge;
    [SerializeField] private Sprite emptyCharge;

    #endregion

    void Start()
    {
        shotCount = maxShotCount;
        flashlightBeam.enabled = false;

        foreach(Image icon in shotIcons)
        {
            icon.sprite = fullCharge;
        }
    }

    void Update()
    {

        if (this.gameObject.activeInHierarchy == true)
        {
            foreach (Image icon in shotIcons)
            {
                icon.gameObject.SetActive(true);
                
            }
        }
        else
        {
            foreach (Image icon in shotIcons)
            {
                icon.gameObject.SetActive(false);
            }
        }

        if (this.gameObject.activeSelf == true && Input.GetButtonDown("Fire1") && shotCount != 0 && !gameManager.isPaused && !player.inShop)
        {
            //ToggleFlashlight();
           StartCoroutine(SpotLightShot());
          
           currentTime = Time.time;
        }
        
    }

    IEnumerator SpotLightShot()
    {
        if (!isCharging && !isFiring)
        {
            isReady = false;
            isCharging = true;
            //player.canMove = false;
            weaponSoundSource.PlayOneShot(spotlightSounds[0]);
            yield return new WaitForSeconds(chargeDuration);
            weaponSoundSource.PlayOneShot(spotlightSounds[1]);
            flashlightHitBox.gameObject.SetActive(true);
            flashlightBeam.enabled = true;
            isCharging = false;
            isFiring = true;
            //player.canRotate = false;
            //Cursor.lockState = CursorLockMode.Locked;
            yield return new WaitForSeconds(shotDuration);
            weaponSoundSource.PlayOneShot(spotlightSounds[2]);
            flashlightHitBox.gameObject.SetActive(false);
            flashlightBeam.enabled = false;
            //Cursor.lockState = CursorLockMode.None;
            isFiring = false;
            isOn = false;
            isReady = true;
            //player.canRotate = true;
            //player.canMove = true;
            shotCount -= 1;
            BatteryUpdate();
            yield break;
        }
    }

    public override void BatteryUpdate()
    {
        for (int i = 0; i < shotIcons.Length; i++)
        {
            if (i < shotCount)
            {
                shotIcons[i].sprite = fullCharge;
            }
            else
            {
                shotIcons[i].sprite = emptyCharge;
            }
        }
    }
}
