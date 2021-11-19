using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class LightTheMidnightLauncher : MonoBehaviourPunCallbacks
{

    #region Private Fields Serialized

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    public byte maxPlayersPerRoom = 4;
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]  private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField] private GameObject progressLabel;
    [SerializeField] private VideoPlayer openingAnim;
    [SerializeField] private GameObject videoObject;
    [SerializeField] private Button playButton;

    #endregion
    
    #region Private Fields


    string gameVersion = "1";
    bool isConnecting;

    #endregion

    #region Photon Callbacks
     public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon", this);
        Debug.Log("PUN Basics Tutorial / Launcher: OnConnectedToMaster() was called by PUN");

        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            
        }
     }
    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;

        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        Debug.Log("${ PhotonNetwork.CurrentRoom.Name} joined!");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            Debug.Log("we Load the room for one");

            PhotonNetwork.LoadLevel("Room for 1");
            PhotonNetwork.IsMessageQueueRunning = true;
        }
    }
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        openingAnim.Prepare();
        openingAnim.loopPointReached += CheckVideo;
        videoObject.SetActive(false);
    }

    private void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    #endregion

    #region Public Methods 

    public void Connect()
    {
        isConnecting = true;

        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Joining Room");
            PhotonNetwork.JoinRandomRoom();
            
        }
        else
        {
            Debug.Log("Connecting");
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
            Debug.Log("Accessing Settings");
        }
    }
    #endregion

    public void ConnectSinglePlayer()
    {
        playButton.interactable = false;
        PhotonNetwork.OfflineMode = true;
        videoObject.SetActive(true);
        openingAnim.Play();

      
        
    }

    void CheckVideo(UnityEngine.Video.VideoPlayer vp) {

        PhotonNetwork.LoadLevel("Vertical Slice");

    }



}
