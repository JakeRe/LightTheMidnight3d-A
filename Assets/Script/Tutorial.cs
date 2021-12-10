using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private AudioSource speaker;
    [SerializeField] private PlayableAsset[] tutorials;
    [SerializeField] private GameObject[] dialogueBoxes;
    [SerializeField] private int dialoguePassed;
    [SerializeField] private int maxDialogue;
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayableDirector playDirect;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        dialoguePassed = 1;
        dialogueBoxes[1].SetActive(false);
        StartCoroutine(CheckDialogue());
        
    }
    public void IncrementDialoguePassed()
    {
        dialoguePassed++;
    }

   IEnumerator CheckDialogue()
    {
        while(dialoguePassed < maxDialogue)
        {
            switch (dialoguePassed)
            {
                case 1:
                    dialogueBoxes[0].SetActive(true);
                    playDirect.playableAsset = tutorials[0];
                    playDirect.Play();
                    yield return new WaitUntil(() => dialoguePassed > 1);
                    break;
                case 2:
                    Debug.Log("Second Dialogue Ready!");
                    if (player.health < player.maxHealth)
                    {
                        dialogueBoxes[1].SetActive(true);
                        playDirect.playableAsset = tutorials[1];
                        playDirect.Play();
                    }
                    break;
                case 3:
                    Debug.Log("Third Dialogue Ready!");
                    if (player.playerPoints > 0)
                    {
                        playDirect.playableAsset = tutorials[2];
                        playDirect.Play();
                    }
                    break;
                default:
                    Debug.Log("No Dialogue has Played");
                    break;
            }
        }
        
    }
}
