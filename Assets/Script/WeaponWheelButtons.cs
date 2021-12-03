using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelButtons : MonoBehaviour
{

    [SerializeField] private Animator buttonAnim;
    [SerializeField] public int Weapon;
    [SerializeField] public string ItemName;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private Image selectedItem;
    [SerializeField] private bool selected;
    [SerializeField] private WeaponManagement weaponManage;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Weapons weapon;

    // Start is called before the first frame update
    void Start()
    {
        buttonAnim = GetComponent<Animator>();
        weapon = weaponPrefab.GetComponent<Weapons>();
        weaponManage = FindObjectOfType<WeaponManagement>();
        Weapon = weapon.weaponID;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Selected()
    {
        selected = true;
        weaponManage.activeWeapon = Weapon;
        weaponManage.selectedWeapon = Weapon;
        weaponManage.SelectedWeapon();
        Debug.Log("Weapon Changed");
    }

    public void Deselected()
    {
        selected = false;
    }

    public void HoverEnter()
    {
        buttonAnim.SetBool("Hover", true);
        itemText.text = ItemName;
    }

    public void HoverExit()
    {
        buttonAnim.SetBool("Hover", false);
        itemText.text = null;
    }
}
