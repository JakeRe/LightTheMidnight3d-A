using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class CameraTracking : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject tPlayer;
    public Transform m_Follow;
    private CinemachineVirtualCamera vcam;
    void Start()
    {

        vcam = GetComponent<CinemachineVirtualCamera>();
    
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if(tPlayer == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (PhotonView.Get(player).IsMine)
                {
                    vcam.Follow = player.transform;
                    DontDestroyOnLoad(this);
                    break;
                }
            }
        }
        
    }
}
