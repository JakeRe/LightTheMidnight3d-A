using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{

    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Collider collider;


    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }


    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Power Up Interacted With");
            mesh.enabled = false;
            collider.enabled = false;
        }
    }
}
