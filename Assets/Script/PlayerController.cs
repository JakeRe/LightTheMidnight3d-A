using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.AI;
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
    [SerializeField] public float baseMovementSpeed;
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
    [SerializeField] public float maxHealth;
    [SerializeField] public float maxHealthAbsolute;
    [Tooltip("How Much Time In Invincibility The Player has After Taking Damage")]
    [SerializeField] public float invincibilityTime;
    [SerializeField] public bool interactedWithPowerUp;
    [Tooltip("This player can be damaged")]
    [SerializeField] public bool canBeDamaged;
    [Tooltip("Alpha of the player material when the player takes damage")]
    [SerializeField] private float damageAlpha;
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Color playerNormal;
    [SerializeField] private Material playerMaterial;
    [SerializeField] public bool inShop;

    public delegate void ChangeHealth();
    public static event ChangeHealth OnHealthChangedPositive;
    public static event ChangeHealth OnHealthChangedNegative;
    public delegate void PickedUp();
    public static event PickedUp PickedUpItem;
    public static event PickedUp inPickUp;
    
    #endregion

    #region Player Point Management
    [Header("Player Point Management")]
    [SerializeField] public float playerPoints;
    [SerializeField] private float debugPoints;
    #endregion

    #region Tutorial Block
    [SerializeField] public Tutorial tutorial;
    #endregion

    #region Delegates for Events 
    public delegate void ActiveRegion();
    public static event ActiveRegion Door;
    public static event ActiveRegion Shop;
    public static event ActiveRegion Disable;
    public static event ActiveRegion PickUp;
    #endregion



    #region Components 
    [Header("Components")]
    [Tooltip("Local player Instance")]
    [SerializeField] public static GameObject LocalPlayerInstance;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator playerAnim;
    #endregion

    #region Sound Effects
    [Header("Sound Effects")]
    [Tooltip("SFX For Player Taking Damage")]
    [SerializeField] private AudioClip[] Sounds;
    [Tooltip("AudioSource Attached to the Player")]
    [SerializeField] private AudioSource playerAS;
    [Tooltip("Audio For Player Movement")]
    [SerializeField] private AudioSource footstepSoundSource;
    [SerializeField] private AudioClip[] steps;
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
            rb = LocalPlayerInstance.GetComponent<Rigidbody>();
            playerCam = GameObject.Find("Main Camera").GetComponent<Camera>();
            tutorial = FindObjectOfType<Tutorial>();
            playerAS = LocalPlayerInstance.GetComponent<AudioSource>();
            playerRenderer = LocalPlayerInstance.GetComponent<Renderer>();
            playerNormal = playerRenderer.material.color;
            playerMaterial = playerRenderer.material;
            playerAnim = GetComponentInChildren<Animator>();
            canBeDamaged = true;
            canMove = true;
            canRotate = true;
            interactedWithPowerUp = false;
            baseMovementSpeed = movementSpeed;
         
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
        {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
       
        }
    void Update()
        {

        //if (Input.GetKey(KeyCode.Tab))
        //{
        //    playerPoints += debugPoints;
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    health = 0;
        //}
      

        //If the photon view is registered as the players then when the health is equal to or less than zero, the player will be sent back to the main menu.

       
        if(health <= 0f){
            canMove = false;
            canRotate = false;
            canBeDamaged = false;
         }
        
        
        }

    void FixedUpdate()
    {
        //This handles the players movement if the photon view is registered as theirs. 
        if (photonView.IsMine && !inShop)
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

    public override void OnEnable()
    {
        base.OnEnable();
        
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
            playerAnim.SetBool("IsWalking", true);
            Step();
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothRotate, smoothDamp);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            rb.MovePosition(transform.position + direction);
        }
        else
        {
            playerAnim.SetBool("IsWalking", false);
            AnimatePlayerIdle(); 
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
                playerAS.PlayOneShot(Sounds[1]);
                
            }

            if (other.gameObject.CompareTag("Health"))
            {
                if(health < maxHealth)
                {
                    Pickups pickedUpItem = other.GetComponent<Pickups>();
                    pickedUpItem.Item();
                    health += 1;
                    playerAS.PlayOneShot(Sounds[3]);
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
        if (other.gameObject.CompareTag("UnlockBarrier") && tutorial.dialogueBoxes[2].gameObject.activeInHierarchy == false && tutorial.currentDialogue > 3)
        {
           

            WorldArea area = other.gameObject.GetComponentInParent<WorldArea>();
            if(area.unlockCanvas != null && !area.isUnlocked)
            {
                Door();
                area.unlockCanvas.enabled = true;
                area.unlockText.enabled = true;
            }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    area.UnlockArea();
                   
                }
        }
        else if (other.gameObject.CompareTag("Shop"))
        {
            if (!inShop)
            {
                Shop();
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                inShop = !inShop;
                canMove = !canMove;
                Debug.Log($"Player in shop = {inShop}");
            }

        }
        else if (other.gameObject.CompareTag("Deployable"))
        {
            inPickUp();
            if (Input.GetKeyUp(KeyCode.E))
            {
                PickUp();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("UnlockBarrier") && other.gameObject != null)
        {
            WorldArea area = other.gameObject.GetComponentInParent<WorldArea>();
            area.unlockCanvas.enabled = false;
            area.unlockText.text = area.defaultText;
        }

        Disable();
    }

    void OnCollisionEnter(Collision enemy)
    {
        if (enemy.gameObject.CompareTag("Enemy") && canBeDamaged && !interactedWithPowerUp)
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

    void AnimatePlayerIdle()
    {
        int RandomNum;
        RandomNum = Random.Range(1, 15);
        if (RandomNum < 5)
        {
            playerAnim.SetTrigger("IdleCautious");
        }
        else if (RandomNum > 10)
        {
            playerAnim.SetTrigger("IdleHeadScratch");
        }
    }

    void Step()
    {
        int step = Random.Range(0, steps.Length);
        if(footstepSoundSource.isPlaying == false)
        {
            footstepSoundSource.PlayOneShot(steps[step]);

        }
    }

}

