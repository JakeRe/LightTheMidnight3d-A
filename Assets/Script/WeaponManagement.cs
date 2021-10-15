using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using Cinemachine;

public class WeaponManagement: MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private int selectedWeapon;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    private void Start()
    {
        SelectedWeapon();
    }

    private void Update()
    {
        int previousWeaponSelection = selectedWeapon;

        if(Input.GetAxis("Mouse ScrollWheel")> 0f)
        {
            if(selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if(selectedWeapon <= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon--;
            }
        }

        if(previousWeaponSelection != selectedWeapon)
        {
            SelectedWeapon();
        }

    }
    void SelectedWeapon()
    {
        int i = 0;

        foreach(Transform weapon in transform)
        {
            if(i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
