using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float shotCount;
    [Tooltip("The maximum amount of shots this weapon can hold")]
    [SerializeField] private float maxShotCount;
    #endregion

    void Start()
    {
        shotCount = maxShotCount;   
    }

    void Update()
    {
        if(this.gameObject.activeSelf == true && Input.GetButtonDown("Fire1") && shotCount != 0)
        {
           ToggleFlashlight();
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
            player.canMove = false;
            weaponSoundSource.PlayOneShot(spotlightSounds[0]);
            yield return new WaitForSeconds(chargeDuration);
            weaponSoundSource.PlayOneShot(spotlightSounds[1]);
            flashLightEmitter.gameObject.SetActive(true);
            flashlightHitBox.gameObject.SetActive(true);
            isCharging = false;
            isFiring = true;
            player.canRotate = false;
            Cursor.lockState = CursorLockMode.Locked;
            yield return new WaitForSeconds(shotDuration);
            weaponSoundSource.PlayOneShot(spotlightSounds[2]);
            flashLightEmitter.gameObject.SetActive(false);
            flashlightHitBox.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            isFiring = false;
            isOn = false;
            isReady = true;
            player.canRotate = true;
            player.canMove = true;
            shotCount -= 1;
            yield break;
        }
    }
}
