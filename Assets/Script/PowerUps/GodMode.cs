using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodMode : PowerUps
{

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(SetInvulnerable());
        }
    }

    IEnumerator SetInvulnerable()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        player.canBeDamaged = false;
        yield return new WaitForSecondsRealtime(duration);
        player.canBeDamaged = true;
        Destroy(this.gameObject);
    }


}
   

