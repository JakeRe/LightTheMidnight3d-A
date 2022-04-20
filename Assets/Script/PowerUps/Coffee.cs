using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coffee : PowerUps
{
    public float speedMultiplier;
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
        player.movementSpeed = player.movementSpeed * speedMultiplier;
        yield return new WaitForSecondsRealtime(30f);
        player.movementSpeed = player.movementSpeed / speedMultiplier;
        Destroy(this.gameObject);
    }

}
