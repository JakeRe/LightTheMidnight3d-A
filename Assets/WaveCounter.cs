using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveCounter : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private WaveSystem wave;
    [SerializeField] private TextMeshProUGUI waveText;


    void Start()
    {
        waveText.text = $"Current Wave: {wave.currentWave}";
    }


    // Update is called once per frame
    void Update()
    {
        waveText.text = $"Current Wave: {wave.currentWave}";
    }
}

    



