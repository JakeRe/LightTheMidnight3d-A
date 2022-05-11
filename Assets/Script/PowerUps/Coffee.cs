using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coffee : PowerUps
{
    public float speedMultiplier;
    public bool multiplied;

    public override void Awake()
    {
        
        base.Awake();
        icon = powerUpManage.icons[4];
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {

            StartCoroutine(IncreaseSpeed());        
        }
    }
    
    IEnumerator IncreaseSpeed()
    {
        
        PlayerController player = FindObjectOfType<PlayerController>();
        Animator playerAnim = player.gameObject.GetComponentInChildren<Animator>();
        if (player.movementSpeed <= player.baseMovementSpeed)
        {
            playerAnim.speed = 2;
            player.movementSpeed = player.movementSpeed * speedMultiplier;
            multiplied = true;
        }
        yield return new WaitForSecondsRealtime(duration);
        player.movementSpeed = player.baseMovementSpeed;
        playerAnim.speed = 1;
        DeactivateIcon();
        Destroy(this.gameObject);
    }

}
