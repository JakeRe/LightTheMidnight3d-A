using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField]
    private float hInput;
    [SerializeField]
    private float vInput;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        transform.Translate(0, 0, vInput * moveSpeed * Time.deltaTime);
        transform.Rotate(0, hInput * rotateSpeed * Time.deltaTime, 0);
    }
}
