using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerName : MonoBehaviour
{
    // Start is called before the first frame update
    #region Private Fields
    [Header("Player Text Information")]
    [Tooltip("Display name for the players.")]
    [SerializeField] private TextMeshProUGUI playerNameText;

    private PlayerController target;
    #endregion

    #region public methods 

    public void SetTarget(PlayerController _target)
    {
        if(_target == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        target = _target;
        if(playerNameText != null)
        {
            playerNameText.text = target.photonView.Owner.NickName;
        }
    }

    #endregion

    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    void Update()
    {
       if(target == null)
        {
            Destroy(this.gameObject);
            return;
        }    
    }

}
