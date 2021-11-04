using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Element for Player Health")]
    [SerializeField] private Image healthUI;
    [SerializeField] public Slider weaponBattery;
    [SerializeField] private Animator healthAnim;

    public delegate void EquippedWeapon();
    public static event EquippedWeapon batteryUpdate;
    //public delegate void ManageBattery();
    //public static event ManageBattery BatteryLevelUpdate;
    private void Awake()
    {
        healthAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerController.OnHealthChangedPositive += HealthChangePositive;
        PlayerController.OnHealthChangedNegative += HealthChangeNegative;
    }

    private void OnDisable()
    {
        PlayerController.OnHealthChangedNegative -= HealthChangeNegative;
        PlayerController.OnHealthChangedPositive -= HealthChangePositive;
    }
    void HealthChangeNegative()
    {
        healthAnim.SetTrigger("HealthRemoved");
    }

    void HealthChangePositive()
    {
        healthAnim.SetTrigger("HealthAdded");
    }

    void BatteryChange()
    {
        
    }
}
