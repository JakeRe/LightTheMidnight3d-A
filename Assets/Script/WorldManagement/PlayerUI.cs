using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using UnityEngine.AI;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Element for Player Health")]
    [SerializeField] public Slider weaponBattery;
    [SerializeField] private PlayerController player;
    [SerializeField] public Sprite emptyBattery;
    [SerializeField] public Sprite fullBattery;

    [SerializeField] private float health;
    [SerializeField] private float numOfHearts;
    [SerializeField] public Image[] hearts;
    [SerializeField] public Image batteryLevel;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;

    [SerializeField] private TextMeshProUGUI playerPoints;
    [SerializeField] public float points;

    [SerializeField] private GameObject interact;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private string DoorText;
    [SerializeField] private string ShopText;
    [SerializeField] private string PickUpText;

    [SerializeField] private TextMeshProUGUI wavesSurvived;
    [SerializeField] private GameManager gameManage;

    public delegate void EquippedWeapon();
    public static event EquippedWeapon batteryUpdate;

    [SerializeField] private WaveSystem wave;

    public static bool isPaused;

   
    private void Awake()
    {
        wave = FindObjectOfType<WaveSystem>();
        gameManage = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
        health = player.health;
        points = player.playerPoints;
        playerPoints.text = points.ToString();
        numOfHearts = health;
        interact.SetActive(false);
        deathMenu.SetActive(false);
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

        Death();
        
    }

    void Death()
    {
        if(player.health == 0)
        {
            gameManage.isPaused = true;
            playerHud.SetActive(false);
            pauseMenu.SetActive(false);
            deathMenu.SetActive(true);
            wavesSurvived.text = $"You Survived {wave.currentWave} Waves";
        }
    }

    private void OnEnable()
    {
        PlayerController.Door += ActiveInteract;
        PlayerController.Shop += ActiveShop;
        PlayerController.Disable += DisableInteract;
        PlayerController.inPickUp += InPickup;
    }

    private void OnDisable()
    {
        PlayerController.Door -= ActiveInteract;
        PlayerController.Shop -= ActiveShop;
        PlayerController.Disable -= DisableInteract;
        PlayerController.inPickUp -= InPickup;
    }

    void ActiveInteract()
    {
        interact.SetActive(true);
        interactText.text = DoorText;
    }

    void ActiveShop()
    {
        interact.SetActive(true);
        interactText.text = ShopText;
    }

    void InPickup()
    {
        interact.SetActive(true);
        interactText.text = PickUpText;
    }

    public void DisableInteract()
    {
        interact.SetActive(false);
    }
}
