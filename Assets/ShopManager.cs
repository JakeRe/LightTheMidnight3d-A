using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using UnityEngine.Playables;


public class ShopManager : MonoBehaviour
{
    // Start is called before the first frame update

    public delegate void OnEnterShop();
    public static event OnEnterShop OnEnteredShop;
    [SerializeField] private PlayerController player;
    [SerializeField] private CinemachineVirtualCamera shopCam;
    [SerializeField] private CinemachineVirtualCamera playCam;
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private Canvas ShopCanvas;
    [SerializeField] private TextMeshProUGUI playerPoints;
    [SerializeField] private float points;
    [SerializeField] private PlayableDirector shopDirector;
    [SerializeField] private PlayableAsset[] transitions;


    void Start()
    {
       player = FindObjectOfType<PlayerController>();
       playerUI = FindObjectOfType<PlayerUI>();
       shopDirector = GetComponent<PlayableDirector>();
       this.points = playerUI.points;
        playerPoints.text = points.ToString();
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
           
            playerUI.gameObject.SetActive(false);
            ShopCanvas.gameObject.SetActive(true);
            shopDirector.playableAsset = transitions[0];
            shopDirector.Play();
            DetectEnemies();
            StartCoroutine(WaitForTIme());
          
        }
        else
        {
            playerUI.gameObject.SetActive(true);
            ShopCanvas.gameObject.SetActive(false);
            shopDirector.playableAsset = transitions[1];
            shopDirector.Play();
            StartCoroutine(WaitForTIme());

            var enemy_in_map = GameObject.FindObjectsOfType<Enemy>();

            foreach (Enemy enemy in enemy_in_map)
            {
                NavMeshAgent enemyNavmesh = enemy.GetComponent<NavMeshAgent>();
                enemyNavmesh.enabled = true;
            }
        }
        StatsUpdated();
       
    }

    void DetectEnemies()
    {
        var enemy_in_map = GameObject.FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemy_in_map)
        {
            NavMeshAgent enemyNavmesh = enemy.GetComponent<NavMeshAgent>();
            enemyNavmesh.enabled = !enemyNavmesh.enabled;
        }
    }

    void StatsUpdated()
    {
        this.points = playerUI.points;
        playerPoints.text = points.ToString();
    }

    IEnumerator WaitForTIme()
    {
        yield return new WaitForSecondsRealtime(.04f);
    }
}
