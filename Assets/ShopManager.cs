using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cinemachine;

public class ShopManager : MonoBehaviour
{
    // Start is called before the first frame update

    public delegate void OnEnterShop();
    public static event OnEnterShop OnEnteredShop;
    [SerializeField] private PlayerController player;
    [SerializeField] private CinemachineVirtualCamera shopCam;
    [SerializeField] private CinemachineVirtualCamera playCam;


    void Start()
    {
       player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        OnShopEntered();
    }

    void OnShopEntered()
    {
        if(player.inShop == true)
        {
            shopCam.gameObject.SetActive(true);
            playCam.gameObject.SetActive(false);

            var enemy_in_map = GameObject.FindObjectsOfType<Enemy>();

            foreach (Enemy enemy in enemy_in_map)
            {
                NavMeshAgent enemyNavmesh = enemy.GetComponent<NavMeshAgent>();
                enemyNavmesh.enabled = false;
            }
        }
        else
        {
            playCam.gameObject.SetActive(true);
            shopCam.gameObject.SetActive(false);

            var enemy_in_map = GameObject.FindObjectsOfType<Enemy>();

            foreach (Enemy enemy in enemy_in_map)
            {
                NavMeshAgent enemyNavmesh = enemy.GetComponent<NavMeshAgent>();
                enemyNavmesh.enabled = true;
            }
        }
       
    }
}
