using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldArea : MonoBehaviour
{
    [SerializeField]
    public float unlockCost;
    [SerializeField]
    public string areaName;
    [SerializeField]
    public Transform[] areaSpawns;
    [SerializeField]
    private GameObject areaEntrance;
    [SerializeField]
    private GameObject areaTrigger;

    [SerializeField]
    private bool isUnlocked;

    WaveSystem WaveManager;

    void Start()
    {
        WaveManager = GameObject.FindGameObjectWithTag("Waves").GetComponent<WaveSystem>();
    }

    void Update()
    {
        
    }

    public void UnlockArea(float playerCurrency)
    {
        if (playerCurrency >= unlockCost)
        {
            Debug.Log("You unlocked " + areaName + "!");
            isUnlocked = true;

            for (int i = 0; i < areaSpawns.Length - 1; i++)
            {
                WaveManager.spawnPoints.Add(areaSpawns[i]);
            }
            Destroy(areaEntrance);
        }
        else
        {
            Debug.Log("Not enough currency!");
        }
    }
}
