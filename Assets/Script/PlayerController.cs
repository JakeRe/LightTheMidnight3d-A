using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("GameObjects")]
    [Tooltip("Hitbox for the flashlight")]
    [SerializeField] private GameObject flashlightHitBox;
    [Tooltip("Main camera for use in navigation")]
    [SerializeField] private Camera mainCamera;
    [Tooltip("The game object associated with the Player")]
    [SerializeField] private GameObject player;
    [Tooltip("The light gameobject used to cast the Flashlight beam")]
    [SerializeField] public Light flashLightEmitter;
    
    [Header("Navigation")]
    [Tooltip("Position of the raycast from the Camera that the player will travel to")]
    [SerializeField] private Vector3 worldPosition;
    [Tooltip("Player's Movement Speed.")]
    [SerializeField] private float movementSpeed;

    [Header("Light Management")]
    [Tooltip("Tells if the flashlight is ready for use")]
    [SerializeField] public bool isReady;
    [Tooltip("Current Battery Level of Flashlight")]
    [SerializeField] public float batteryLevel;
    [Tooltip("Maximum Battery Life for Flashlight")]
    [SerializeField] public float batteryLevelMax;
    [Tooltip("Rate by which the battery for the flashlight drains")]
    [SerializeField] private float batteryDrain; 
    [Tooltip("Color emitted by flashlight")]
    [SerializeField] private Color lightColor;
    [Tooltip("Flashlight maximum range")]
    [SerializeField] public float maxFlashlightRange;
    
    void Start()
    {
        isReady = true;
        flashLightEmitter.gameObject.SetActive(false);
        flashlightHitBox.SetActive(false);
    }

    //Coroutine that deactivates and recharges the light.
    IEnumerator FlashLightCoolDown()
    {
        //Light waits for five seconds before recharging.
        yield return new WaitForSeconds(5);
        //If the battery is not ready and less than 100
        if(batteryLevel < 100 && !isReady)
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

    // Update is called once per frame
    void Update()
    {
        Movement();
        Attack();
        BatteryManagement();
    }

    //Region containing player motion and abilities
    #region Player Abilities
    void Attack()
    {
        if (Input.GetButton("Fire1") && batteryLevel >= 0 && isReady)
        {
            flashLightEmitter.gameObject.SetActive(true);
            batteryLevel = batteryLevel -= batteryDrain * Time.deltaTime;
            flashlightHitBox.gameObject.SetActive(true);

            flashLightEmitter.color -= (Color.white / batteryDrain) * Time.deltaTime;
            flashLightEmitter.range -= flashLightEmitter.range * Time.deltaTime;

        }
    }


    void Movement()
    {
        Vector3 mouse = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            worldPosition = hit.point;

        }

        Debug.DrawRay(transform.position, mouse, Color.green);

        player.transform.LookAt(worldPosition);


        if (Input.GetKey(KeyCode.W))
        {
            player.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        }
    }


    void BatteryManagement()
    {
        if (batteryLevel <= 0)
        {
            isReady = false;
            flashlightHitBox.gameObject.SetActive(false);
            flashLightEmitter.gameObject.SetActive(false);
        }

        if (batteryLevel <= batteryLevelMax && !isReady)
        {
            StartCoroutine("FlashLightCoolDown");
        }
    }

    #endregion
}

