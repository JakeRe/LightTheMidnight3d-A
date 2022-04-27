﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{

    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Collider collider;
    [SerializeField] protected float duration;



    private void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        collider = GetComponentInChildren<Collider>();
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