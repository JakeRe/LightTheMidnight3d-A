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
    #region Variables
    [Tooltip("The Player Active in the Scene")]
    [SerializeField] private PlayerController player;
    [Tooltip("The player's user interface")]
    [SerializeField] private PlayerUI playerUI;
    [Tooltip("The canvas used to display the shop's menu")]
    [SerializeField] private Canvas ShopCanvas;
    [Tooltip("Text element that displays the players points")]
    [SerializeField] private TextMeshProUGUI playerPoints;
    [Tooltip("The amount of points the player has")]
    [SerializeField] private float points;
    [Tooltip("Playable director for camera control")]
    [SerializeField] private PlayableDirector shopDirector;
    [Tooltip("Array of playable assets that are accessed by the shop director")]
    [SerializeField] private PlayableAsset[] transitions;
    #endregion

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
