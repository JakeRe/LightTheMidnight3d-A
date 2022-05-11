using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GodMode : PowerUps
{
    [SerializeField] private Material godMode;

    public override void Awake()
    {
        base.Awake();
        icon = powerUpManage.icons[0];

    }


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
        PlayerUI playerUI = FindObjectOfType<PlayerUI>();
        PlayerController player = FindObjectOfType<PlayerController>();
        player.canBeDamaged = false;
        player.interactedWithPowerUp = true;
        foreach(Image image in playerUI.hearts)
        {
            image.material = godMode;
        }
        yield return new WaitForSecondsRealtime(duration);
        player.canBeDamaged = true;
        player.interactedWithPowerUp = false;
        foreach (Image image in playerUI.hearts)
        {
            image.material = null;
        }
        DeactivateIcon();
        Destroy(this.gameObject);
    }

    

}
   

