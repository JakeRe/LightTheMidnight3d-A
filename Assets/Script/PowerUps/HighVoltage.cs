using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighVoltage : PowerUps
{
    [SerializeField] private MeshRenderer[] batteryRenderer;

    public override void Awake()
    {
        
        base.Awake();
        icon = powerUpManage.icons[3];
    }


    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            batteryRenderer[0].enabled = false;
            batteryRenderer[1].enabled = false;
            batteryRenderer[2].enabled = false;

            StartCoroutine(ActivateHighVoltage());
        }
    }


    IEnumerator ActivateHighVoltage()
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            var weapon_active = FindObjectsOfType<Weapons>();
            foreach (Weapons weapon in weapon_active)
            {
                weapon.damageRate += Mathf.Infinity;
            }
            yield return null;
            currentTime += Time.deltaTime;
        }
        var weapons = FindObjectsOfType<Weapons>();
        foreach(Weapons weapon in weapons)
        {
            weapon.damageRate = weapon.defaultDamage;
        }
        Destroy(this.gameObject);
    }
}
