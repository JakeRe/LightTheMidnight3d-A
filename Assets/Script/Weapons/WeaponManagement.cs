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
    [SerializeField] public int activeWeapon;

    //[SerializeField] public delegate void WeaponWheel();
    //[SerializeField] public static event WeaponWheel OnActive;
  
    public const byte WeaponSwitchEventCode = 1;
    private void Awake()
    {
        //weaponWheel.SetActive(false);
        playerController = GetComponentInParent<PlayerController>();
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
        //WeaponWheelControl();

    }

    
    public void SelectedWeapon()
    {
            foreach (GameObject weapon in weaponObjects)
            {

                if (selectedWeapon == weapon.GetComponent<Weapons>().weaponID)
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
            activeWeapon++;
            }
        

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(WeaponSwitchEventCode, activeWeapon, raiseEventOptions, SendOptions.SendReliable);
    }

    public void WeaponChange()
    {
        int previousWeaponSelection = selectedWeapon;
        if (playerController.photonView.IsMine)
        {
            activeWeapon = 0;
           

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
            
        }
        if (previousWeaponSelection != selectedWeapon)
        {
            SelectedWeapon();
        }

    }

    //void WeaponWheelControl()
    //{
    //    if(weaponWheel != null)
    //    {
    //        if (Input.GetKey(KeyCode.Q))
    //        {
    //            weaponWheel.SetActive(true);
    //            Time.timeScale = .5f;
    //        }
    //        else
    //        {
    //            weaponWheel.SetActive(false);
    //            Time.timeScale = 1f;
    //        }
    //    }
      
    //}

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
