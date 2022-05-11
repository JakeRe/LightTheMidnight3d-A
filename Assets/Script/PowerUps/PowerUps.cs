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
    [SerializeField] protected GameObject icon;
    [SerializeField] protected TextMeshProUGUI timerText;

    [SerializeField] protected PowerUpManagement powerUpManage;
    private IEnumerator despawn;

    


    public virtual void Awake()
    {
        powerUpManage = FindObjectOfType<PowerUpManagement>();
        mesh = GetComponentInChildren<MeshRenderer>();
        collider = GetComponentInChildren<Collider>();
        powerUpAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if(icon != null)
        {
            icon.SetActive(false);
            timerText = icon.GetComponentInChildren<TextMeshProUGUI>();
        }
        despawn = Despawn();
        StartCoroutine(despawn);
    }

    public void OnEnable()
    {
        PowerUpManagement.PowerUpActivated += timeStart;
    }

    public void OnDisable()
    {
        PowerUpManagement.PowerUpActivated -= timeStart;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopCoroutine(despawn);
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

    void timeStart()
    {
        StartCoroutine(Timer());
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
        DeactivateIcon();
        yield return null;
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSecondsRealtime(20f);
        Debug.Log("Destroyed Power up");
        Destroy(this.gameObject);
    }

    protected void DeactivateIcon()
    {
        icon.SetActive(false);
    }
}
