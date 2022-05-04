using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PowerUps : MonoBehaviour
{

    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Collider collider;
    [SerializeField] protected float duration;
    
    [SerializeField] private AudioSource powerUpAudio;
    [SerializeField] private AudioClip powerUpClip;
    [SerializeField] private GameObject icon;
    [SerializeField] private TextMeshProUGUI timerText;


    private void Awake()
    {
        icon.SetActive(false);
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
            icon.SetActive(true);
            powerUpAudio.PlayOneShot(powerUpClip);
        }
    }
}
