using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float movementSpeed;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Light flashLightEmitter;
    [SerializeField] private float batteryLevel;
    [SerializeField] private float batteryLevelMax;
    [SerializeField] private float batteryDrain;
    [SerializeField] private GameObject player;
    [SerializeField] private bool isReady;
    [SerializeField] private GameObject flashlightHitBox;
    [SerializeField] private Vector3 worldPosition;


    void Start()
    {
        isReady = true;
        flashLightEmitter.gameObject.SetActive(false);
        flashlightHitBox.SetActive(false);
    }

    IEnumerator FlashLightCoolDown()
    {
        
        yield return new WaitForSeconds(5);
        if(batteryLevel < 100 && !isReady)
        {
            batteryLevel += batteryDrain * Time.deltaTime;

            if (batteryLevel >= batteryLevelMax)
            {
                batteryLevel = batteryLevelMax;
                isReady = true;
                yield break;
            }
        }

        



    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
       
        if (Physics.Raycast(ray, out hit,1000))
        {
            worldPosition = hit.point;
          
        }

        Debug.DrawRay(transform.position, mouse, Color.green);

        player.transform.LookAt(worldPosition);
       

        if (Input.GetKey(KeyCode.W))
        {
            player.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);  
        }

        if (Input.GetButton("Fire1") && batteryLevel >= 0 && isReady)
        {
            flashLightEmitter.gameObject.SetActive(true);
            batteryLevel = batteryLevel -= batteryDrain * Time.deltaTime;
            flashlightHitBox.gameObject.SetActive(true); 
            
            
        }

        if(batteryLevel <= 0)
        {
            isReady = false;
            flashlightHitBox.gameObject.SetActive(false);
            flashLightEmitter.gameObject.SetActive(false);
        }

        if (batteryLevel <= batteryLevelMax && !isReady)
        {
            StartCoroutine("FlashLightCoolDown");
        }

    }
}
