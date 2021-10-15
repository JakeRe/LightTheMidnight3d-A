using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPowerUp : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
      
    }
}
