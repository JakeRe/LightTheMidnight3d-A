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

    [SerializeField]
    public float darkCurrency;
    [SerializeField]
    public float health;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("UnlockBarrier"))
        {
            UnlockTransactionEnter(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("UnlockBarrier"))
        {
            AreaUnlock(other.gameObject);
        }
        
    }

    // All of this area unlocking stuff should really be moved to the WorldArea script.
    // I can also create a general "unlock" or "purchase" script that'll handle transactions for
    // unlocking areas, shop purchases, and anything else you need to buy. Then the actual
    // area unlocking would be within the World Area script (removing the barrier, adding spawns, etc.)
    private void UnlockTransactionEnter(GameObject area)
    {
        WorldArea areaScript = area.GetComponentInParent<WorldArea>();

        Debug.Log("Do you want to unlock " + areaScript.areaName + "? Press Space to unlock, and ESC to cancel.");
    }
    private void AreaUnlock(GameObject area)
    { 
        WorldArea areaScript = area.GetComponentInParent<WorldArea>();

        //Debug.Log("Do you want to unlock " + areaScript.areaName + "? Press Space to unlock, and ESC to cancel.");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            areaScript.UnlockArea(darkCurrency);
            if (areaScript.unlockCost <= darkCurrency)
                darkCurrency -= areaScript.unlockCost;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Cancelled unlock.");
        }

    }
}
