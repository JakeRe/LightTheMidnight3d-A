using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
public class PlayerController : MonoBehaviour, IPunObservable
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

    [Header("Health Management")]
    [Tooltip("Health of the Player Character")]
    [SerializeField] public float health;

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
    [Tooltip("If the flashlight is in Use")]
    [SerializeField] public bool isActive;

    [Header("Components")]
    [Tooltip("Photon Viewer")]
    [SerializeField] private PhotonView photonView;
    [Tooltip("Local player Instance")]
    public static GameObject LocalPlayerInstance;



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(isActive);
        }
        else
        {
            this.health = (float)stream.ReceiveNext();
            this.isActive = (bool)stream.ReceiveNext();
        }

    }

    void Awake()
    { 
        photonView = gameObject.GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;

        }

       

        DontDestroyOnLoad(this.gameObject);
    }

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
        if(photonView.IsMine && PhotonNetwork.IsConnected == true)
        {
            Movement();
            Attack();
            BatteryManagement();

            if(health < 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }
        
    }

    //Region containing player motion and abilities
    #region Player Abilities
    void Attack()
    {
        if (photonView.IsMine)
        {
            if (Input.GetButton("Fire1") && batteryLevel >= 0 && isReady)
            {
                isActive = true;
                flashLightEmitter.gameObject.SetActive(true);
                batteryLevel = batteryLevel -= batteryDrain * Time.deltaTime;
                flashlightHitBox.gameObject.SetActive(true);

                flashLightEmitter.color -= (Color.white / batteryDrain) * Time.deltaTime;
                flashLightEmitter.range -= flashLightEmitter.range * Time.deltaTime;

            }
        }
       
    }


    void Movement()
    {
        //Vector3 mouse = Input.mousePosition;

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, 1000))
        //{
        //    worldPosition = hit.point;

        //}

        //Debug.DrawRay(transform.position, mouse, Color.green);

        //player.transform.LookAt(worldPosition);


        if (Input.GetKey(KeyCode.W))
        {
            player.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            player.transform.Translate(-Vector3.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            player.transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            player.transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("enemy hit player");

        }
        else
        {
            //Do Nothing
        }

    }

    
}

