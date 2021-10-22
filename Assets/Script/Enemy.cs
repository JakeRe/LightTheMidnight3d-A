using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float moveSpeed;

    public NavMeshAgent enemyAgent;

    void Start()
    {
        
    }

    void Update()
    {
        MoveTowardTarget(TargetPosition("Player"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HitBox"))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private Vector3 TargetPosition(string targetTag)
    {
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);
        return target.transform.position;
    }

    private void MoveTowardTarget(Vector3 targetPos)
    {
        enemyAgent.SetDestination(targetPos);
    }
}
