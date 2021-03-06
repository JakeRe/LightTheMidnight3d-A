using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldArea : MonoBehaviour
{
    [Tooltip("The cost to unlock this area.")]
    [SerializeField] public float unlockCost;
    [Tooltip("The name of the area to be used by UI elements.")]
    [SerializeField] public string areaName;
    [SerializeField] public Transform[] areaSpawns;

    [SerializeField] private PlayerController player;


    // The physical barrier preventing the player from entering an area.
    [Tooltip("The Game Object with a collider that prevents entities from entering this area.")]
    [SerializeField] private GameObject areaEntrance;

    // The collider that triggers unlock UI while the player is within it.
    [Tooltip("The Game Object with a trigger that will activate UI elements when the player is nearby.")]
    [SerializeField] private GameObject areaTrigger;

    // The barrier particle system.
    [Tooltip("The Game Object holding the Particle System that is used to visualize the locked area.")]
    [SerializeField] private GameObject areaParticles;

    [Tooltip("A bool that indicates if the area is unlocked or not.")]
    [SerializeField] public bool isUnlocked;
    [SerializeField] public bool navUpdated;

    [SerializeField] public Canvas unlockCanvas;
    [SerializeField] public TextMeshProUGUI unlockText;
    public string defaultText;

    WaveSystem WaveManager;
    [SerializeField] NavMeshSurfaceManager NavManager;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
      
        unlockCanvas = GetComponentInChildren<Canvas>();
        unlockText = GetComponentInChildren<TextMeshProUGUI>();
        
        WaveManager = GameObject.FindGameObjectWithTag("Waves").GetComponent<WaveSystem>();
        //NavManager = GameObject.FindGameObjectWithTag("Nav").GetComponent<NavMeshSurfaceManager>();
        isUnlocked = false;
        defaultText = string.Format("Would you like to unlock the {0} area using {1} Obscuraplasm?", areaName, unlockCost.ToString());
        unlockCanvas.enabled = false;
        unlockText.text = defaultText;
    }

    void Update()
    {
        
    }

    public void UnlockArea()
    {

        if (!isUnlocked)
        {
            Debug.Log("Area is locked.");
           

            if (player.playerPoints >= unlockCost)
            {
                // Display successful purchase dialogue from Dr.
                unlockText.text = string.Format("Welcome to the {0} area!", areaName);
                Debug.Log("Unlock successful!");
                isUnlocked = true;

                Debug.Log("Player Points Spent");

                 player.playerPoints -= unlockCost;

                Debug.Log($"Player Currency is : {player.playerPoints}");

                for (int i = 0; i < areaSpawns.Length; i++)
                {
                    WaveManager.spawnPoints.Add(areaSpawns[i]);
                }

                unlockCanvas.enabled = false;
                Destroy(areaParticles.gameObject);
                Destroy(areaEntrance.gameObject);
            }
            else
            {
                // Display "not enough points" dialogue from Dr.
                unlockText.text = string.Format($"Fool! You need {unlockCost} Obscuraplasm to bypass this barrier!");
                Debug.Log("Not enough points.");
            }

        }


    }
}
