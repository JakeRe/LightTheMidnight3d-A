using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Game Objects
    [Header("GameObjects")]
    [Tooltip("Hitbox for the flashlight")]
    [SerializeField] private GameObject flashlightHitBox;
    [Tooltip("Main camera for use in navigation")]
    [SerializeField] private Camera mainCamera;
    [Tooltip("The game object associated with the Player")]
    [SerializeField] private GameObject player;
    [Tooltip("The light gameobject used to cast the Flashlight beam")]
    [SerializeField] public Light flashLightEmitter;
    [Tooltip("This is the player's UI Prefab")]
    [SerializeField] public GameObject PlayerUIPrefab;
    #endregion
    #region Navigation Management
    [Header("Navigation")]
    [Tooltip("Position of the raycast from the Camera that the player will travel to")]
    [SerializeField] private Vector3 worldPosition;
    [Tooltip("Player's Movement Speed.")]
    [SerializeField] public float movementSpeed;
    [Tooltip("Stored Vector 3 for movement")]
    [SerializeField] private Vector3 movementControl = Vector3.zero;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [Tooltip("Virtual Camera used for Navigation")]
    [SerializeField] private Transform playerCam;
    [Tooltip("Value used to smooth out the rotational movemnent")]
    [SerializeField] private float smoothDamp = 0.1f;
    [SerializeField] private float smoothRotate;

    #endregion 
    #region Player Health Management
    [Header("Health Management")]
    [Tooltip("Health of the Player Character")]
    [SerializeField] public float health;
    #endregion
    #region Light Control
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
    #endregion
    #region Components 
    [Header("Components")]
    [Tooltip("Local player Instance")]
    [SerializeField] public static GameObject LocalPlayerInstance;
    [SerializeField] private CharacterController characterController;
    #endregion 

    #region Unity Callbacks
    void Awake()
        { 
      
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
         {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

        //CameraWorkLTM _cameraWork = this.gameObject.GetComponent<CameraWorkLTM>();


        //if (_cameraWork != null)
        //{
        //    if (photonView.IsMine)
        //    {
        //        _cameraWork.OnStartFollowing();
        //    }
        //}

        if (PlayerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUIPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

        isReady = true;
        flashLightEmitter.gameObject.SetActive(false);
        flashlightHitBox.SetActive(false);
         }
    void Update()
        {
        if(photonView.IsMine)
        {
            
            //this.Attack();
            //this.BatteryManagement();

            if(this.health < 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }
        else
        {
            //DoNothing
        }
        
        }

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            this.Movement();
        }
    }
    #endregion

    #region Photon Calls
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
            stream.SendNext(isActive);
            
           
        }
        else if(stream.IsReading)
        {
            this.health = (float)stream.ReceiveNext();
            this.isActive = (bool)stream.ReceiveNext();
            
        }

    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this.gameObject;
        object[] instantiationData = info.photonView.InstantiationData;

       
    }




    #endregion

    #region Unity Scene Management
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }

    void OnLevelWasLoaded(int level)
    {
        this.CalledOnLevelWasLoaded(level);
    }

    void CalledOnLevelWasLoaded(int level)
    {
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

        GameObject _uiGo = Instantiate(this.PlayerUIPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    #endregion

    #region Enumerators 
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
    #endregion

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
        
        
         horizontal = Input.GetAxis("Horizontal") * movementSpeed;
         vertical = Input.GetAxis("Vertical") * movementSpeed;
        
        
         
         Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothRotate, smoothDamp);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            characterController.Move((Vector3.right * horizontal + Vector3.forward * vertical) * Time.deltaTime);
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

        if (batteryLevel <= batteryLevelMax && !isReady)
        {
            StartCoroutine("FlashLightCoolDown");
        }
    }
    #endregion

    #region Colission Detection
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        else
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("enemy hit player");

            }
        }
    }

    #endregion

}

