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
    [Tooltip("Main camera for use in navigation")]
    [SerializeField] private Camera mainCamera;
    [Tooltip("The game object associated with the Player")]
    [SerializeField] private GameObject player;
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

    #region Player Point Management
    [Header("Player Point Management")]
    [SerializeField] private float Points;
    #endregion

    #region Components 
    [Header("Components")]
    [Tooltip("Local player Instance")]
    [SerializeField] public static GameObject LocalPlayerInstance;
    [Tooltip("Player controller component attached to player prefab")]
    [SerializeField] private CharacterController characterController;
    #endregion

    #region Weapons Management
    [Header("Weapons Management")]
    [Tooltip("Weapons Management Script Attached to Weapon Manager Prefab")]
    [SerializeField] private WeaponManagement weaponManagement;
    [Tooltip("What the active weapon is for the player")]
    [SerializeField] public Weapons activeWeapon;
    #endregion

    #region Unity Callbacks
    void Awake()
        { 

        //If the photon view component registers as the users, then it makes the local player instance this game object 
        //Then it checks for the weapon manager in it's children.
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
            weaponManagement = LocalPlayerInstance.GetComponentInChildren<WeaponManagement>();
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

        //This is for player UI instantiation, if the UI prefab isn't null, then it will print a debug message. If not, then it will send a message that sets the target. 

        if (PlayerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUIPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

       
        }
    void Update()
        {

        //If the photon view is registered as the players then when the health is equal to or less than zero, the player will be sent back to the main menu.

        if(photonView.IsMine)
        {
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
        //This handles the players movement if the photon view is registered as theirs. 
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
           
            
           
        }
        else if(stream.IsReading)
        {
            this.health = (float)stream.ReceiveNext();
           
            
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

    #region Player Abilities

    //This is how the player moves using the WASD directions on their keyboard. 
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
        else
        {
            
        }
    }

    #endregion

    #region Colission Detection
    private void OnTriggerEnter(Collider other)
    {
        //This differentiates for the player if it's their photon view that hits another object.
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

            if (other.gameObject.CompareTag("Battery"))
            {
                //When the player hits a battery pickup, this sets the active weapon's battery level to maximum. Then destroys the battery
                activeWeapon.batteryLevel = activeWeapon.batteryLevelMax;
                Destroy(other.gameObject);
            }
        }
    }

    #endregion

}

