using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldArea : MonoBehaviour
{
    [Tooltip("The cost to unlock this area.")]
    [SerializeField] public float unlockCost;
    [Tooltip("The name of the area to be used by UI elements.")]
    [SerializeField] public string areaName;
    [SerializeField] public Transform[] areaSpawns;

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

    WaveSystem WaveManager;
    [SerializeField] NavMeshSurfaceManager NavManager;

    void Start()
    {
        WaveManager = GameObject.FindGameObjectWithTag("Waves").GetComponent<WaveSystem>();
        //NavManager = GameObject.FindGameObjectWithTag("Nav").GetComponent<NavMeshSurfaceManager>();
        isUnlocked = false;
    }

    void Update()
    {

    }

    public void UnlockArea(float playerCurrency)
    {
        if (!isUnlocked)
        {
            Debug.Log("Area is locked.");
            // Display randomized unlock dialogue, including price and area name.

            if (playerCurrency >= unlockCost)
            {
                // Display successful purchase dialogue from Dr.
                Debug.Log("Unlock successful!");
                isUnlocked = true;

                for (int i = 0; i < areaSpawns.Length; i++)
                {
                    WaveManager.spawnPoints.Add(areaSpawns[i]);
                }

                Destroy(areaParticles);
                Destroy(areaEntrance);
            }
            else
            {
                // Display "not enough points" dialogue from Dr.
                Debug.Log("Not enough points.");
            }

        }
    }
}
