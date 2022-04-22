using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSandwhich : PowerUps
{
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.health = player.maxHealth;
        }
        Destroy(this.gameObject);
    }
}
