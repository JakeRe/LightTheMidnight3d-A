﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float attackRate;
    [SerializeField]
    private int enemyValue;
    [SerializeField]
    private GameObject deathFX;
    [SerializeField]
    private GameObject enemyModel;
    private bool isDead;
    private bool givePoints = true;
    private bool oddsCalced = false;

    /// <summary>
    /// The following fields are used to manage the health of the Enemy,
    /// as well as the UI elements that display the health.
    /// </summary>
    [SerializeField] public float currentHealth;
    [SerializeField] public float maxHealth;
    [SerializeField] public float baseMaxHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject enemyUI;
    [SerializeField] private Camera uiCam;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private int minOdds;
    [SerializeField] private int maxOdds;
    [SerializeField] private GameObject[] powerUps;

    #region Sounds
    [Header("Sound Materials")]
    [SerializeField] public AudioSource roakSoundSource;
    [SerializeField] private AudioClip[] roakGrowlSounds;
    [SerializeField] private AudioClip[] roakAttackSounds;
    [SerializeField] private AudioClip takingDamage;
    [SerializeField] private float minRandomSoundTime;
    [SerializeField] private float maxRandomSoundTime;
    [SerializeField] private bool isplaying;
    [SerializeField] private float damageTime;
    [SerializeField] public bool isTargetable;
    [SerializeField] public int soundIndex;
    [SerializeField] private float spaceBetweenSounds;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    #endregion

    public NavMeshAgent enemyAgent;

    [Header("Animations")]
    //[SerializeField] private Animator enemyAnimator;
    [SerializeField] private MeshRenderer[] roakSkin;

    private TrackTrigger triggerEvent;


    void Start()
    {
        isTargetable = true;
        boxCollider = GetComponent<BoxCollider>();
        roakSoundSource = GetComponent<AudioSource>();
        deathFX.GetComponentInChildren<ParticleSystem>();
        //enemyAnimator = GetComponentInChildren<Animator>();
       

        roakSoundSource.enabled = true;

        currentHealth = maxHealth;
        uiCam = Camera.main;
        givePoints = true;
        triggerEvent = new TrackTrigger(gameObject);
    }

   
    void Update()
    {

        StartCoroutine(SoundPause());

        if (!CloseToPlayer())
         {
                MoveTowardTarget(TargetPosition("Player"));
         }
         else if (CloseToPlayer())
         {
                MoveTowardTarget(TargetPosition(null));
         }
        
       

        enemyUI.transform.rotation = uiCam.transform.rotation;
        healthSlider.value = HealthBarCheck();
        CheckHealth();
    }

    private void FixedUpdate()
    {
        triggerEvent.FixedUpdate();
    }

    private void OnTriggerStay(Collider other)
    {
        triggerEvent.TriggerUpdate(other);
        // If Inside of Weapon Hitbox
        if (other.gameObject.CompareTag("HitBox"))
        {
            Weapons weaponScript = other.gameObject.GetComponentInParent<Weapons>();
            currentHealth -= weaponScript.damageRate;
            roakSoundSource.clip = takingDamage;

            if(roakSoundSource.isPlaying == false)
            {
                roakSoundSource.Play();
            }
        }
        else
        {
            roakSoundSource.clip = roakGrowlSounds[0];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEvent.AddTrigger(other, OnTriggerExit);
        // Check if player's weapon trigger
       
    }

    private void OnTriggerExit(Collider other)
    {
        triggerEvent.RemoveTrigger(other);
        // Check if player's weapon trigger
        if (other.gameObject.CompareTag("HitBox"))
        {
            // Reset to growl sound
            roakSoundSource.clip = roakGrowlSounds[0];
        }
    }

    private void SetRenderers(GameObject parent, bool newState) 
    {
        // Turn on/off all renderers
        var renderers = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (newState)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
            else
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
    }


    private bool CloseToPlayer()
    {
        // Check if the distance between this object and the Player is less than or equal to the attack range.
        if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= attackRange)
        {
            //enemyAnimator.SetTrigger("Attack");
            return true;
        }

        return false;
    }

    private Vector3 TargetPosition(string targetTag)
    {
        if (targetTag == null)
            return gameObject.transform.position;

        // Find target object in scene and return its position.
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);

        // In case the target does not exist, return self
        if(target == null)
            return this.transform.position;

        return target.transform.position;
    }

    private void MoveTowardTarget(Vector3 targetPos)
    {
        if(enemyAgent.enabled == true)
        {
            enemyAgent.SetDestination(targetPos);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawLine(transform.position, TargetPosition("Player"));
    }

    private void CheckHealth()
    {
        if (currentHealth == maxHealth)
        {
            enemyUI.SetActive(false);
        }
        else if (currentHealth < maxHealth)
        {
            enemyUI.SetActive(true);
        }

        if (currentHealth <= 0)
        {
            roakSoundSource.Stop();
            boxCollider.enabled = false;
            MoveTowardTarget(TargetPosition(null));
            enemyUI.SetActive(false);
            deathFX.GetComponent<ParticleSystem>().Play();
            roakSkin[0].enabled = false;
            roakSkin[1].enabled = false;
            StartCoroutine(SpawnPowerUp());
            Destroy(gameObject, 1f);
            isDead = true;
        }
        if (isDead && givePoints)
        {
            PlayerController playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
            playerScript.playerPoints += enemyValue;
            givePoints = false;
        }
    }

    float HealthBarCheck()
    {
        return currentHealth / maxHealth;
    }

    IEnumerator SoundPause()
    {
        bool hasPlayedRecently;
        hasPlayedRecently = false;
        yield return new WaitForSecondsRealtime(Random.Range(minRandomSoundTime, maxRandomSoundTime));
        if (roakSoundSource.isPlaying == false && !hasPlayedRecently)
        {
            roakSoundSource.clip = roakGrowlSounds[Random.Range(0, roakGrowlSounds.Length)];
            roakSoundSource.pitch = Random.Range(minPitch, maxPitch);
            roakSoundSource.Play();
            hasPlayedRecently = true;
        }
        yield return new WaitForSecondsRealtime(spaceBetweenSounds);
        hasPlayedRecently = false;
    }

    IEnumerator SpawnPowerUp()
    {
        if (!oddsCalced)
        { 
            int odds = Random.Range(minOdds, maxOdds);
            Debug.Log(odds);
            if (odds < 50)
            {
                oddsCalced = true;
                //do nothing 
            }
            else if (odds >= 50 && odds <= 60)
            {
                oddsCalced = true;

                Instantiate(powerUps[0], transform.position, transform.rotation);
            }
            else if (odds >= 61 && odds <= 70)
            {
                oddsCalced = true;

                Instantiate(powerUps[1], this.transform);
            }
            else
            {
                oddsCalced = true;
            }
        }

        yield break;


    }
}
