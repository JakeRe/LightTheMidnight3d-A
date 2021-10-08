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
        if(other.CompareTag("Player"))
        {
            player.batteryLevel = player.batteryLevelMax;
            player.isReady = true;
            player.flashLightEmitter.color = Color.white;
            player.flashLightEmitter.range = player.maxFlashlightRange;
            Destroy(this.gameObject);
        }
    }
}
