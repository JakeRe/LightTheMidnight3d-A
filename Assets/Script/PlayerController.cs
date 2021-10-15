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

