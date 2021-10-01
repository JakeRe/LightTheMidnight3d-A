using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Photon Callbacks


    public override void OnPlayerEnteredRoom(Player other)
    {

        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

            LoadArena();
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
    public static GameManager Instance;
    public GameObject playerPrefab;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 5;
        Instance = this;

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("MultiplayerLauncher");

            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab has not been set. Add Player Prefab to Game Manager in the Inspector");
        }
        else
        {
            if (PlayerController.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 1);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);

            }
        }
    }
    #endregion

    #region Public Methods
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MultiplayerLauncher");
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
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
    
  

}
