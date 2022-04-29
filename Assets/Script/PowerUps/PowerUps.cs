using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PowerUps : MonoBehaviour
{

    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Collider collider;
    [SerializeField] protected float duration;
    
    [SerializeField] private AudioSource powerUpAudio;
    [SerializeField] private AudioClip powerUpClip;


    private void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        collider = GetComponentInChildren<Collider>();
        powerUpAudio = GetComponent<AudioSource>();
    }


    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Power Up Interacted With");
            mesh.enabled = false;
            collider.enabled = false;
            powerUpAudio.PlayOneShot(powerUpClip);
        }
    }
}
