using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class AirTrafficMelee : Weapons
{
    // Start is called before the first frame update

    [SerializeField] private Animator laserAnim;
    
    void Start()
    {
        laserAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ToggleFlashlight();
    }

    public override void ToggleFlashlight()
    {
        if (Input.GetButton("Fire1"))
        {
            laserAnim.SetBool("IsAttacking", true);
            
        }
        
        if (Input.GetButtonUp("Fire1"))
        {
            laserAnim.SetBool("IsAttacking", false);
        }
    }

    public override void BatteryUpdate()
    {
        
    }
}
