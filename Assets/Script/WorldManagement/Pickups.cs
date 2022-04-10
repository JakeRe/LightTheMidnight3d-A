using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    // Start is called before the first frame update

    private Renderer pickupRend;
    private Material pickupMat;
    private Color defaultColor;
    [SerializeField] private float alpha;
    [SerializeField] private bool activeAndReady;
    [SerializeField] private float coolDown;

    private void Awake()
    {
        pickupRend = GetComponent<Renderer>();
        pickupMat = pickupRend.material;
        defaultColor = pickupMat.color;
    }


    private void OnEnable()
    {
        PlayerController.PickedUpItem += Item;
    }

    private void OnDisable()
    {
        PlayerController.PickedUpItem -= Item;
    }

    public void Item()
    {
        StartCoroutine(PickedUp());
    }

    IEnumerator PickedUp()
    {
        activeAndReady = !activeAndReady;
        PickupActive();
        Debug.Log("Entered Coroutine");
        yield return new WaitForSecondsRealtime(coolDown);
        activeAndReady = !activeAndReady;
        PickupActive();
        yield break;

    }

    public void PickupActive()
    {
        if (this.activeAndReady == false)
        {
            pickupMat.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, alpha);
        }
        else
        {
            pickupMat.color = defaultColor;
        }
    }
}
