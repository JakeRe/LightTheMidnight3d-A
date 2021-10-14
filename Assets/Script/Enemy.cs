using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject Player;

    public float moveSpeed;

    public NavMeshAgent enemyAgent;

    void Start()
    {
        
    }

    void Update()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        // Random movement to get the objects moving around the scene - testing purposes only.
        // float step = moveSpeed * Time.deltaTime;
        // transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
        enemyAgent.SetDestination(Player.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HitBox"))
        {
            Destroy(this.gameObject);
        }
        else
        {

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
