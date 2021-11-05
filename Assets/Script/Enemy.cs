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
    [SerializeField]
    private float attackRate;

    public NavMeshAgent enemyAgent;

    //public enum EnemyState { CHASING, ATTACKING, COOLDOWN }
    //public EnemyState state;

    void Start()
    {

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HitBox"))
        {
            Destroy(this.gameObject);
        }
    }

   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }*/

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

    /*private void Attack()
    {
        TestPlayer playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<TestPlayer>();
        playerScript.health -= 1;
    }*/

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawLine(transform.position, TargetPosition("Player"));
    }
}
