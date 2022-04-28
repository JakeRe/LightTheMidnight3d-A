using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coffee : PowerUps
{
    public float speedMultiplier;
    public bool multiplied;
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
        if (player.movementSpeed <= player.baseMovementSpeed)
        {
            player.movementSpeed = player.movementSpeed * speedMultiplier;
            multiplied = true;
        }
        yield return new WaitForSecondsRealtime(duration);
        player.movementSpeed = player.baseMovementSpeed;
        Destroy(this.gameObject);
    }

}
