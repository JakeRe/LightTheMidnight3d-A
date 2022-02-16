using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Element for Player Health")]
    [SerializeField] public Slider weaponBattery;
    [SerializeField] private PlayerController player;
    [SerializeField] public Sprite emptyBattery;
    [SerializeField] public Sprite fullBattery;

    [SerializeField] private float health;
    [SerializeField] private float numOfHearts;
    [SerializeField] private Image[] hearts;
    [SerializeField] public Image batteryLevel;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private TextMeshProUGUI playerPoints;
    [SerializeField] public float points;

    public delegate void EquippedWeapon();
    public static event EquippedWeapon batteryUpdate;

    public static bool isPaused;
   
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        health = player.health;
        points = player.playerPoints;
        playerPoints.text = points.ToString();
        pauseMenu.SetActive(false);
        isPaused = false;
        playerHud.SetActive(true);
        numOfHearts = health;
    }

    private void Update()
    {
        numOfHearts = player.maxHealth;
        points = player.playerPoints;
        health = player.health;
        playerPoints.text = points.ToString();


        for (int i = 0; i<hearts.Length; i++)
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            Pause();
        }
    }


    public void Pause()
    {
        if (isPaused == true)
        {
            Time.timeScale = 0;
            playerHud.SetActive(false);
            pauseMenu.SetActive(true);
        }

        if (!isPaused)
        {
            Time.timeScale = 1;
            playerHud.SetActive(true);
            pauseMenu.SetActive(false);
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
