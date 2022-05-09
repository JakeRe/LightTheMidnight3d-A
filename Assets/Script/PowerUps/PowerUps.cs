using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PowerUps : MonoBehaviour
{

    [SerializeField] protected MeshRenderer mesh;
    [SerializeField] private Collider collider;
    [SerializeField] protected float duration;
    [SerializeField] protected float timer;
    
    [SerializeField] private AudioSource powerUpAudio;
    [SerializeField] private AudioClip powerUpClip;
    [SerializeField] private GameObject icon;
    [SerializeField] protected TextMeshProUGUI timerText;


    private void Awake()
    {
        icon.SetActive(false);
        mesh = GetComponentInChildren<MeshRenderer>();
        collider = GetComponentInChildren<Collider>();
        powerUpAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(Despawn());
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Power Up Interacted With");
            if(mesh != null)
            {
                mesh.enabled = false;
            }
            collider.enabled = false;
            if(icon != null)
            {
                icon.SetActive(true);
            }
            powerUpAudio.PlayOneShot(powerUpClip);
            if(duration != 0)
            {
                timer = duration;
                StartCoroutine(Timer());
            }
        }
    }

    IEnumerator Timer()
    {
        while (timer > 0)
        {
            timerText.text = timer.ToString();
            timer -= 1;
            Mathf.Round(timer);
            yield return new WaitForSecondsRealtime(1f);
        }
        yield return null;
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSecondsRealtime(20f);
        Destroy(this.gameObject);
    }
}
