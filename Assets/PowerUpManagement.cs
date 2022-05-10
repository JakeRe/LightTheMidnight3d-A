using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PowerUpManagement : MonoBehaviour
{

    public delegate void PowerUpActive();
    public static event PowerUpActive PowerUpActivated;
    [SerializeField] public GameObject[] icons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
