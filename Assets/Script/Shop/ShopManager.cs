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
    [SerializeField] protected PlayerController player;
    [Tooltip("The player's user interface")]
    [SerializeField] private PlayerUI playerUI;
    [Tooltip("The canvas used to display the shop's menu")]
    [SerializeField] private Canvas ShopCanvas;
    [Tooltip("Text element that displays the players points")]
    [SerializeField] protected TextMeshProUGUI playerPoints;
    [Tooltip("The amount of points the player has")]
    [SerializeField] protected float points;
    [Tooltip("Playable director for camera control")]
    [SerializeField] private PlayableDirector shopDirector;
    [Tooltip("Array of playable assets that are accessed by the shop director")]
    [SerializeField] private PlayableAsset[] transitions;
    [SerializeField] protected float cost;
    [SerializeField] private ParticleSystem rain;
    [SerializeField] private AudioSource shopAudio;
    [SerializeField] private GameObject mainGameAudio;
    [SerializeField] private AudioSource mainGameAudioSource;
    #endregion

    protected void Start()
    {
       //This locates all of the components that are required for methods
       player = FindObjectOfType<PlayerController>();
       playerUI = FindObjectOfType<PlayerUI>();
       shopDirector = GetComponent<PlayableDirector>();
       points = player.playerPoints;
       playerPoints.text = points.ToString();
       rain.Play();
       mainGameAudio = GameObject.FindGameObjectWithTag("SceneMusic");
       mainGameAudioSource = mainGameAudio.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        OnShopEntered();
        StatsUpdated();

    }

    #region Opening and Closing
    /// <summary>
    ///  When the player presses the Interact key while standing inside of the shop collider the following happens
    ///  The player UI is turned off 
    ///  The shop UI is turned on
    ///  The playable director plays the transition going from the player camera to the shop camera
    ///  Enemies are then detected and there is a brief waiting period for the director to finish
    ///  
    ///  If the player is already inside, it does the inverse and activates the nav meshes of the enemies so that the 
    ///  game may continue.
    /// </summary>
    void OnShopEntered()
    {
        if(player.inShop == true)
        {
            rain.Stop();
            mainGameAudioSource.Pause();
            if(shopAudio.isPlaying == false)
            {
                shopAudio.Play();
            }
            playerUI.gameObject.SetActive(false);
            ShopCanvas.gameObject.SetActive(true);
            shopDirector.playableAsset = transitions[0];
            shopDirector.Play();
            var enemy_in_map = GameObject.FindObjectsOfType<Enemy>();

            foreach (Enemy enemy in enemy_in_map)
            {
                enemy.enemyAgent.isStopped = true;
                AudioSource enemyAudio = enemy.GetComponent<AudioSource>();
                enemyAudio.Pause();
            }
            StartCoroutine(WaitForTIme());
          
        }
        else if(player.inShop == false)
        {
            if (rain.isStopped)
            {
                rain.Play();
            }

            if(mainGameAudioSource.isPlaying == false)
            {
                mainGameAudioSource.Play();

            }
            shopAudio.Stop();
            playerUI.gameObject.SetActive(true);
            ShopCanvas.gameObject.SetActive(false);
            shopDirector.playableAsset = transitions[1];
            shopDirector.Play();
            StartCoroutine(WaitForTIme());

            var enemy_in_map = GameObject.FindObjectsOfType<Enemy>();

            foreach (Enemy enemy in enemy_in_map)
            {
                enemy.enemyAgent.isStopped = false;
                AudioSource enemyAudio = enemy.GetComponent<AudioSource>();
                enemyAudio.Play();
            }
        }
        StatsUpdated();
       
    }

    //This keeps the UI for the player's points updated.
    protected void StatsUpdated()
    {
        points = player.playerPoints;
        playerPoints.text = points.ToString();
    }

    IEnumerator WaitForTIme()
    {
        yield return new WaitForSecondsRealtime(.04f);
    }
    #endregion
}
