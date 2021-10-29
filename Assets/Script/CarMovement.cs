using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public Transform carGO;
    public float moveDistance;

    public Transform armGO;
    public Transform rotationPoint;
    public float rotationDistance;

    void Start()
    {
        RotateBarrier();
    }

    void Update()
    {
        
    }

    public void MoveCar()
    {
        Vector3 moveVec = new Vector3(moveDistance, 0, 0);
        carGO.transform.Translate(moveVec);
    }

    public void RemoveBarrier()
    {
        BoxCollider carBox = carGO.GetComponent<BoxCollider>();
        carBox.enabled = false;
    }

    public void RotateBarrier()
    {
        armGO.transform.Translate(0.55f, 0, 0);
        armGO.transform.Rotate(0, 0, rotationDistance, Space.Self);
    }
}
