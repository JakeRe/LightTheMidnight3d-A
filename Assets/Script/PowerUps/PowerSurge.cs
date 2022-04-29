using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSurge : PowerUps
{
    [SerializeField] private Animator surgeAnim;
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroyAllEnemies());
        }
    }
 
    IEnumerator DestroyAllEnemies()
    {
        var enemy_in_map = FindObjectsOfType<Enemy>();
        surgeAnim.SetTrigger("Interacted");
        yield return new WaitForSecondsRealtime(2);
        foreach (Enemy enemy in enemy_in_map)
        {
            enemy.currentHealth = 0;
        }
        yield return new WaitForSecondsRealtime(3);
        Destroy(this.gameObject);
    }
}
