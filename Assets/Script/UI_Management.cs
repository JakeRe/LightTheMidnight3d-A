using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Management : MonoBehaviour
{
    public GameObject WaveText;
    void Start()
    {

    }

    void Update()
    {
        WaveSystem waveScript = GameObject.FindGameObjectWithTag("Waves").GetComponent<WaveSystem>();
        Text waveText = WaveText.GetComponent<Text>();
        waveText.text = "Wave: " + waveScript.currentWave;
    }
}
