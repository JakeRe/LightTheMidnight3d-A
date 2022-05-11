using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.AI;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Photon Callbacks


    public override void OnPlayerEnteredRoom(Player other)
    {

        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            PhotonNetwork.IsMessageQueueRunning = false;
            LoadArena();
            PhotonNetwork.IsMessageQueueRunning = true;

        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

            LoadArena();
        }

    }

    #endregion

    #region Public Fields 
    static public GameManager Instance;
    public GameObject playerPrefab;
    public const int SpawnLocalPlayer = 1;
    public LightTheMidnightLauncher LTML;
    public bool altPause;
    public VideoPlayer ending;
    public RawImage endingScreen;
    public GameObject soundManager;
    public WeaponManagement weaponManage;





    private GameObject instance;
    #endregion

    #region Pause Management

    public bool isPaused;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject playerHud;
    [SerializeField] private PlayerController controller;
    [SerializeField] private GameObject player;
    [SerializeField] private WaveSystem waveSys;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        weaponManage = FindObjectOfType<WeaponManagement>();
        PhotonNetwork.AutomaticallySyncScene = true;
        endingScreen.gameObject.SetActive(false);
        ending.Prepare();
        ending.loopPointReached += CheckVideo;
        pauseMenu.SetActive(false);
        playerHud.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player");
        waveSys = FindObjectOfType<WaveSystem>();
        controller = player.GetComponent<PlayerController>();
        isPaused = false;
    }
    private void Start()
    {
        Instance = this;


        if (PhotonNetwork.OfflineMode == false)
        {
            //if (!PhotonNetwork.IsConnected)
            //{
            //    PhotonNetwork.LoadLevel("MultiplayerLauncher");

            //    return;
            //}

            if (playerPrefab == null)
            {
                Debug.LogError("Player prefab has not been set. Add Player Prefab to Game Manager in the Inspector");
            }
            else
            {
                if (PlayerController.LocalPlayerInstance == null && PhotonNetwork.PlayerList.Length < LTML.maxPlayersPerRoom)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);

                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);

                }
            }
        }
    }

    private void Update()
    {
        Pause();
        CheckForEnding();
    }

    void CheckForEnding()
    {
        if(waveSys.currentWave >= 11)
        {
            altPause = true;
            StartCoroutine(AltPause());
            weaponManage.gameObject.SetActive(false);
            endingScreen.gameObject.SetActive(true);
            soundManager.SetActive(false);
            UIPause();
            playerHud.SetActive(false);
            pauseMenu.SetActive(false);
            ending.Play();
        }
    }


    #endregion

    #region Public Methods
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MultiplayerLauncher");
    }

    public void ReturnToMenuOffline()
    {
        waveSys.ResetHealth();
        PhotonNetwork.LoadLevel("MultiplayerLauncher");
        Destroy(player);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Restart()
    {
        waveSys.ResetHealth();

        PhotonNetwork.LoadLevel("Vertical Slice");

        Destroy(player);
    }



    #endregion

    #region Private Methods 

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork: Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);

    }



    #endregion

    #region Pause Management
    public void Pause()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            UIPause();
            if (isPaused)
            {
                playerHud.SetActive(false);
                pauseMenu.SetActive(true);
                StartCoroutine(PauseEnemies());

            }
            if (!isPaused)
            {
                StopCoroutine(PauseEnemies());
                playerHud.SetActive(true);
                pauseMenu.SetActive(false);
                controller.canMove = true;
                controller.canRotate = true;
            }
        }

    }

    public void EnemyPause()
    {
        altPause = true;
        StartCoroutine(AltPause());
    }

    public void EnemyUnpause()
    {
        altPause = false;
        controller.canMove = true;
        controller.canRotate = true;
        
    }

    public void UIPause()
    {
        isPaused = !isPaused;
       
    }

    void CheckVideo(UnityEngine.Video.VideoPlayer vp)
    {
        waveSys.ResetHealth();
        Destroy(player);
        PhotonNetwork.LoadLevel("MultiplayerLauncher");

    }
    #endregion

    IEnumerator PauseEnemies()
    {
        while (isPaused)
        {
            controller.canMove = false;
            controller.canRotate = false;
            var enemy_in_map = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemy_in_map)
            {
                enemy.enemyAgent.isStopped = true;
                AudioSource audio = enemy.roakSoundSource;
                audio.enabled = false;
            }
            yield return null;
        }
        var enemies_in_map = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies_in_map)
        {
            AudioSource audio = enemy.roakSoundSource;
            audio.enabled = true;
        }
    }

    IEnumerator AltPause()
    {
        while (altPause)
        {
            controller.canMove = false;
            controller.canRotate = false;
            var enemy_in_map = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemy_in_map)
            {
                enemy.enemyAgent.isStopped = true;
                AudioSource audio = enemy.roakSoundSource;
                audio.enabled = false;
            }
            yield return null;
        }
        var enemies_in_map = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies_in_map)
        {
            AudioSource audio = enemy.roakSoundSource;
            audio.enabled = true;
        }
    }
}
