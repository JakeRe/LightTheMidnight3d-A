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
    [SerializeField] public Slider weaponBattery;
    [SerializeField] private PlayerController player;

    [SerializeField] private float health;
    [SerializeField] private float numOfHearts;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    public delegate void EquippedWeapon();
    public static event EquippedWeapon batteryUpdate;
    //public delegate void ManageBattery();
    //public static event ManageBattery BatteryLevelUpdate;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        health = player.health;
        numOfHearts = health;
    }

    private void Update()
    {
        health = player.health;

        for(int i = 0; i<hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    //private void OnEnable()
    //{
    //    PlayerController.OnHealthChangedPositive += HealthChangePositive;
    //    PlayerController.OnHealthChangedNegative += HealthChangeNegative;
    //}

    //private void OnDisable()
    //{
    //    PlayerController.OnHealthChangedNegative -= HealthChangeNegative;
    //    PlayerController.OnHealthChangedPositive -= HealthChangePositive;
    //}

}
