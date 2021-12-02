using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Cinemachine;

public class WeaponManagement: MonoBehaviour, IPunObservable, IOnEventCallback
{
    [Header("GameObjects")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] public int selectedWeapon;
    [SerializeField] private int equipmentCount;
    [SerializeField] private int equipmentMax;
    [SerializeField] private GameObject equipment;
    [SerializeField] public List<GameObject> weaponObjects;
    [SerializeField] private PhotonView weaponPV;

    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private string Name;
    [SerializeField] public Image selectedItem;
    [SerializeField] private bool selected = false;
    [SerializeField] private GameObject weaponWheel;
    [SerializeField] private Animator weaponWheelAnim;

    
  
    public const byte WeaponSwitchEventCode = 1;
    private void Awake()
    {
        weaponWheelAnim = weaponWheel.GetComponent<Animator>();
        weaponWheel.SetActive(false);
        playerController = GetComponent<PlayerController>();
        weaponPV = playerController.GetComponent<PhotonView>();
        foreach(Transform weapon in transform)
        {
            weaponObjects.Add(weapon.gameObject);
        }
    }
    private void Start()
    {
        SelectedWeapon();
    }

    private void Update()
    {
        WeaponChange();
        WeaponWheel();

    }

    
    public void SelectedWeapon()
    {
        int i = 0;

        foreach(Transform weapon in transform)
        {
            if(i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                playerController.activeWeapon = weapon.gameObject.GetComponent<Weapons>();
                Weapons activeWeapon = weapon.gameObject.GetComponent<Weapons>();
                activeWeapon.BatteryUpdate();

            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(WeaponSwitchEventCode, i, raiseEventOptions, SendOptions.SendReliable);

    }

    public void WeaponChange()
    {
        if (playerController.photonView.IsMine)
        {
            int previousWeaponSelection = selectedWeapon;

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedWeapon >= transform.childCount - 1)
                {
                    selectedWeapon = 0;
                }
                else
                {
                    selectedWeapon++;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {

                if (selectedWeapon <= transform.childCount - 1)
                {
                    selectedWeapon = 0;
                }
                else
                {
                    selectedWeapon--;
                }
            }
            if (previousWeaponSelection != selectedWeapon)
            {
                SelectedWeapon();
            }
        }

    }

    public void WeaponWheel()
    {
        if(weaponWheel != null)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                weaponWheel.SetActive(true);
            }
            else
            {
                weaponWheel.SetActive(false);
            }
        }
      
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(selectedWeapon);
            //foreach(GameObject obj in weaponObjects)
            //{
            //    stream.SendNext(obj.activeSelf);
            //}
        }
        if (stream.IsReading)
        {
            selectedWeapon = (int)stream.ReceiveNext();
            //foreach (GameObject obj in weaponObjects)
            //{
            //    obj.SetActive((bool)stream.ReceiveNext());
            //}
        }
    }

    void DeployEquipment()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (equipmentCount >= equipmentMax && equipment != null)
            {
                equipmentCount--;
                
            }
        }
       
    }

    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        PlayerUI.batteryUpdate -= SelectedWeapon;
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == WeaponSwitchEventCode)
        {
            int i = (int)photonEvent.CustomData;
            i = selectedWeapon;

            
        }

    }

    

}
