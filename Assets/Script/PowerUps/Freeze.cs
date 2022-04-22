using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : PowerUps
{
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FreezeEnemies());
        }
    }

    IEnumerator FreezeEnemies()
    {
        float currentTime = 0;
        while(currentTime < duration)
        {
            var enemy_in_map = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemy_in_map)
            {
                enemy.enemyAgent.isStopped = true;
            }
            yield return null;
            currentTime += Time.deltaTime;
            
        }
       

    }
}
