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

    /// <summary>
    /// The following fields are used to manage the health of the Enemy,
    /// as well as the UI elements that display the health.
    /// </summary>
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject enemyUI;
    [SerializeField] private Camera uiCam;

    [Header("Sound Materials")]
    [SerializeField] private AudioSource roakSoundSource;
    [SerializeField] private AudioClip[] roakGrowlSounds;
    [SerializeField] private AudioClip takingDamage;
    [SerializeField] private bool isplaying;
    [SerializeField] private float damageTime;

    public NavMeshAgent enemyAgent;

    void Start()
    {
        roakSoundSource = GetComponent<AudioSource>();
        deathFX.GetComponentInChildren<ParticleSystem>();
        currentHealth = maxHealth;
        uiCam = Camera.main;
        givePoints = true;
    }

    void Update()
    {
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("HitBox"))
        {
            Weapons weaponScript = other.gameObject.GetComponentInParent<Weapons>();
            //Destroy(this.gameObject);
            //StartCoroutine(PlaySound());
            currentHealth -= weaponScript.damageRate;
        }
    }

    IEnumerator PlaySound()
    {
        roakSoundSource.PlayOneShot(takingDamage);
        yield return new WaitForSeconds(damageTime);
        roakSoundSource.PlayOneShot(takingDamage);
    }

    private bool CloseToPlayer()
    {
        // Check if the distance between this object and the Player is less than or equal to the attack range.
        if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= attackRange)
        {
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

        return target.transform.position;
    }

    private void MoveTowardTarget(Vector3 targetPos)
    {
        enemyAgent.SetDestination(targetPos);
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
            MoveTowardTarget(TargetPosition(null));
            enemyUI.SetActive(false);
            deathFX.GetComponent<ParticleSystem>().Play();
            enemyModel.GetComponent<MeshRenderer>().enabled = false;
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
}
