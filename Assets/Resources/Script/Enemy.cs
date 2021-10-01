using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject Player;

    public float moveSpeed;
    void Start()
    {
        
    }

    void Update()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        // Random movement to get the objects moving around the scene - testing purposes only.
        // transform.Translate(new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3)) * Time.deltaTime * moveSpeed);
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, step);
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
