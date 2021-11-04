using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float attackRange;

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
        // Find target object in scene.
        GameObject target = GameObject.FindGameObjectWithTag(targetTag);

        // Check if enemy is within attacking range. If not, return the position of the target.
        if (Vector3.Distance(transform.position, target.transform.position) > attackRange)
            return target.transform.position;

        return transform.position;
    }

    private void MoveTowardTarget(Vector3 targetPos)
    {
        enemyAgent.SetDestination(targetPos);
    }

    private void Attack()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawLine(transform.position, TargetPosition("Player"));
    }
}
