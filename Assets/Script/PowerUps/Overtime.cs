using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overtime : PowerUps
{

    public override void Awake()
    {
       
        base.Awake(); 
        icon = powerUpManage.icons[1];
    }


    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(MultiplyPoints());
        }
    }


    IEnumerator MultiplyPoints()
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            var enemy_in_map = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemy_in_map)
            {
                enemy.multiplierIsActive = true;
            }
            yield return null;
            currentTime += Time.deltaTime;
        }

        var enemies_in_map = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies_in_map)
        {
            enemy.multiplierIsActive = false;
        }
        DeactivateIcon();
        Destroy(this.gameObject);
    }
}
