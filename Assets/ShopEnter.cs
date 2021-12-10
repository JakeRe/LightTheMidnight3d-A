using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ShopEnter : MonoBehaviour
{
    [SerializeField] private PlayableDirector shopDirector;
    [SerializeField] private PlayableAsset[] shopCutscenes;
    [SerializeField] private bool insideShop;
    [SerializeField] private PlayerController player;

    private void Start()
    {
        shopDirector = GetComponent<PlayableDirector>();
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        insideShop = player.inShop;
        CheckPlayerInShop();
    }


    void CheckPlayerInShop()
    {
       
      
    }

    IEnumerator WaitForCutsceneToFinish()
    {
        yield return new WaitUntil(() => insideShop == false);
    }
    public void InsideShop()
    {
        shopDirector.playableAsset = shopCutscenes[1];
        insideShop = !insideShop;
    }

}
