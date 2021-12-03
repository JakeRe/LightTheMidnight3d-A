using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerName : MonoBehaviour
{
    // Start is called before the first frame update
    #region Private Fields
    [Header("Player Text Information")]
    [Tooltip("Display name for the players.")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [Tooltip("Offset from the player target.")]
    [SerializeField] private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    float characterControllerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup _canvasGroup;
    Vector3 targetPosition;

    private PlayerController target;
    #endregion

    #region public methods 

    public void SetTarget(PlayerController _target)
    {
        if(PhotonNetwork.OfflineMode == false)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            target = _target;

            targetTransform = this.GetComponent<Transform>();
            targetRenderer = this.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }

            if (playerNameText != null)
            {
                if (target.photonView.Owner.NickName != null && PhotonNetwork.OfflineMode != true)
                {
                    playerNameText.text = target.photonView.Owner.NickName;

                }
                else{

                }
            }
        }
       
    }

    #endregion

    void Awake()
    {
        
        this.transform.SetParent(GameObject.Find("LIght The Midnight UI").GetComponent<Transform>(), false);

        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    void Update()
    {
       if(target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        
    }

    private void LateUpdate()
    {
        if(targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }

        if(targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }

}
