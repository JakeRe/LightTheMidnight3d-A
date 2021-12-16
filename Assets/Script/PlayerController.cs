using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;
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

    private Weapons weapon;
    #endregion
    #region Navigation Management
    [Header("Navigation")]
    [Tooltip("Position of the raycast from the Camera that the player will travel to")]
    [SerializeField] private Vector3 worldPosition;
    [Tooltip("Player's Movement Speed.")]
    [SerializeField] public float movementSpeed;
    [Tooltip("Stored Vector for Player Rotation")]
    [SerializeField] private Vector3 movementControl = Vector3.zero;
    [SerializeField] private float horizontal;
    [SerializeField] private float vertical;
    [Tooltip("Virtual Camera used for Navigation")]
    [SerializeField] private Camera playerCam;
    [Tooltip("Value used to smooth out the rotational movemnent")]
    [SerializeField] private float smoothDamp = 0.1f;
    [SerializeField] private float smoothRotate;
    [Tooltip("How fast the player rotates in accordance with mouse location")]
    [SerializeField] private float sensitivity;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] public bool canMove;
    [SerializeField] public bool canRotate;
    #endregion 
    #region Player Health Management
    [Header("Health Management")]
    [Tooltip("Health of the Player Character")]
    [SerializeField] public float health;
    [SerializeField] private float maxHealth;
    [Tooltip("How Much Time In Invincibility The Player has After Taking Damage")]
    [SerializeField] public float invincibilityTime;
    [Tooltip("This player can be damaged")]
    [SerializeField] private bool canBeDamaged;
    [Tooltip("Alpha of the player material when the player takes damage")]
    [SerializeField] private float damageAlpha;
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Color playerNormal;
    [SerializeField] private Material playerMaterial;
    public delegate void ChangeHealth();
    public static event ChangeHealth OnHealthChangedPositive;
    public static event ChangeHealth OnHealthChangedNegative;
    public delegate void PickedUp();
    public static event PickedUp PickedUpItem;
    
    #endregion

    #region Player Point Management
    [Header("Player Point Management")]
    [SerializeField] public float playerPoints;
    #endregion

    #region Components 
    [Header("Components")]
    [Tooltip("Local player Instance")]
    [SerializeField] public static GameObject LocalPlayerInstance;
    [SerializeField] private Rigidbody rb;
    #endregion

    #region Sound Effects
    [Header("Sound Effects")]
    [Tooltip("SFX For Player Taking Damage")]
    [SerializeField] private AudioClip[] Sounds;
    [Tooltip("AudioSource Attached to the Player")]
    [SerializeField] private AudioSource playerAS;
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
            weaponManagement = LocalPlayerInstance.GetComponent<WeaponManagement>();
            rb = LocalPlayerInstance.GetComponent<Rigidbody>();
            playerCam = GameObject.Find("Main Camera").GetComponent<Camera>();
            playerAS = LocalPlayerInstance.GetComponent<AudioSource>();
            playerRenderer = LocalPlayerInstance.GetComponent<Renderer>();
            playerNormal = playerRenderer.material.color;
            playerMaterial = playerRenderer.material;
            canBeDamaged = true;
            canMove = true;
            canRotate = true;
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

        //if (PlayerUIPrefab != null && PhotonNetwork.OfflineMode != true)
        //{
        //    GameObject _uiGo = Instantiate(PlayerUIPrefab);
        //    _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        //}
        //else
        //{
        //    Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        //}

       
        }
    void Update()
        {

        //If the photon view is registered as the players then when the health is equal to or less than zero, the player will be sent back to the main menu.

        if (photonView.IsMine)
        {
           
            if (this.health < 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }
        else
        {
            //DoNothing
        }
        
        }

    void FixedUpdate()
    {
        //This handles the players movement if the photon view is registered as theirs. 
        if (photonView.IsMine)
        {
            if (canMove)
                this.Movement();
            if (playerCam != null)
            {
                if (canRotate)
                    Aim();
            }
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
        weaponManagement.enabled = true;

       
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

        //GameObject _uiGo = Instantiate(this.PlayerUIPrefab);
        //_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
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


        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        direction = direction.normalized * movementSpeed * Time.deltaTime;

        if (direction.magnitude >= 0.01f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothRotate, smoothDamp);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            rb.MovePosition(transform.position + direction);
        }
        else
        {
            
        }
        
    }

    void Aim()
    {
        var (success, position) = GetMousePosition();

        if (success)
        {
            // Calculate the direction
            var direction = position - transform.position;

            // Ignore the height difference
            direction.y = 0;

            // Make the transform look in the direction
            transform.forward = direction;
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = playerCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            // The Raycast hit something, return with the position.
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // The Raycast did not hit anything.
            return (success: false, position: Vector3.zero);
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
            if (other.gameObject.CompareTag("Battery"))
            {
                //When the player hits a battery pickup, this sets the active weapon's battery level to maximum. Then destroys the battery
                Pickups pickedUpItem = other.GetComponent<Pickups>();
                pickedUpItem.Item();
                activeWeapon.batteryLevel = activeWeapon.batteryLevelMax;
                activeWeapon.flashLightEmitter.range = activeWeapon.maxFlashlightRange;
                playerAS.PlayOneShot(Sounds[1]);
                
            }

            if (other.gameObject.CompareTag("Health"))
            {
                if(health < maxHealth)
                {
                    Pickups pickedUpItem = other.GetComponent<Pickups>();
                    pickedUpItem.Item();
                    health += 1;
                    OnHealthChangedPositive();
                }
                

            }
        }

        //Used for area unlocking.
        /*if (other.gameObject.CompareTag("UnlockBarrier"))
        {
            UnlockTransactionEnter(other.gameObject);
        }*/
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("UnlockBarrier"))
        {
            Debug.Log("Standing in unlock area.");
            WorldArea area = other.gameObject.GetComponentInParent<WorldArea>();
            area.unlockCanvas.enabled = true;
            
            if (DoesPlayerInteract())
                area.UnlockArea(playerPoints);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("UnlockBarrier"))
        {
            WorldArea area = other.gameObject.GetComponentInParent<WorldArea>();
            area.unlockCanvas.enabled = false;
            area.unlockText.text = area.defaultText;
        }
    }

    void OnCollisionEnter(Collision enemy)
    {
        if (enemy.gameObject.CompareTag("Enemy") && canBeDamaged)
        {
            Debug.Log("enemy hit player");
            health -= 1;
            playerAS.PlayOneShot(Sounds[0]);
            StartCoroutine(Invincibility());
        }
    }

    IEnumerator Invincibility()
    {
        canBeDamaged = !canBeDamaged;
        ChangeAlpha();
        yield return new WaitForSecondsRealtime(invincibilityTime);
        canBeDamaged = !canBeDamaged;
        ChangeAlpha();
        yield break;
    }

    #endregion


    void ChangeAlpha()
    {
        if (!canBeDamaged)
        {
            playerMaterial.color = new Color(playerNormal.r, playerNormal.g, playerNormal.b, damageAlpha);
           
        }
        else
        {
            playerMaterial.color = playerNormal;
        }
     
    }

    private bool DoesPlayerInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Pressed *Interact*");
            return true;
        }

        return false;
    }
}

