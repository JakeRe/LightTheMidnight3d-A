using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
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

    private GameObject instance;
    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        Instance = this;
        if(PhotonNetwork.OfflineMode == false)
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LoadLevel("MultiplayerLauncher");

                return;
            }

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
    
    #endregion

    #region Public Methods
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MultiplayerLauncher");
    }

    public void ReturnToMenuOffline()
    {
        PhotonNetwork.LoadLevel("MultiplayerLauncher");
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    //[PunRPC]
    //public void SpawnPlayer()
    //{
    //    GameObject player = Instantiate(playerPrefab);
    //    PhotonView photonView = player.GetComponent<PhotonView>();

    //    if (PhotonNetwork.AllocateViewID(photonView))
    //    {
    //        object[] data = new object[]
    //        {
    //            player.transform.position, player.transform.rotation, photonView.ViewID
    //        };

    //        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
    //        {
    //            Receivers = ReceiverGroup.Others,
    //            CachingOption = EventCaching.AddToRoomCache
    //        };

    //        SendOptions sendOptions = new SendOptions
    //        {
    //            Reliability = true
    //        };

    //        PhotonNetwork.RaiseEvent(SpawnLocalPlayer, data, raiseEventOptions, sendOptions);
    //    }
    //    else
    //    {
    //        Debug.Log("Failed to Allocate a View ID");
    //        Destroy(player);

    //    }
    //}

    //public void OnEvent(EventData photonEvent)
    //{
    //    if (photonEvent.Code == SpawnLocalPlayer)
    //    {
    //        object[] data = (object[])photonEvent.CustomData;

    //        GameObject player = (GameObject)Instantiate(playerPrefab, (Vector3)data[0], (Quaternion)data[1]);
    //        PhotonView photonView = player.GetComponent<PhotonView>();
    //        photonView.ViewID = (int)data[2];
    //    }
    //}

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
    
  

}
