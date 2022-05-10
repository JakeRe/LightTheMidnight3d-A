using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : PowerUps
{
    [SerializeField] private Material frozenMat;
    [SerializeField] private Material baseMat;

    public override void Awake()
    {
       
        base.Awake(); 
        icon = powerUpManage.icons[2];
    }


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
                Animator animator = enemy.GetComponentInChildren<Animator>();
                animator.speed = 0;
                SkinnedMeshRenderer roakSkin = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
                roakSkin.material = frozenMat;
            }
            yield return null;
            currentTime += Time.deltaTime;
            
        }
        var enemies_in_map = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies_in_map)
        {
            Animator animator = enemy.GetComponentInChildren<Animator>();
            animator.speed = 1;
            SkinnedMeshRenderer roakSkin = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
            roakSkin.material = baseMat;
        }
        
        Destroy(this.gameObject);
       

    }
}
