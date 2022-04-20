using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSurge : PowerUps
{
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            DestroyAllEnemies();
        }
    }

    void DestroyAllEnemies()
    {
        var enemy_in_map = FindObjectsOfType<Enemy>();

        foreach(Enemy enemy in enemy_in_map)
        {
            enemy.currentHealth = 0;
        }
        Destroy(this.gameObject);
    }
}
