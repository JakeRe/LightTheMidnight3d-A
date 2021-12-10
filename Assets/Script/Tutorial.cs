using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Tutorial : MonoBehaviour
{
    [Header("Arrays for Tutorial Timelines and Dialogue")]
    [SerializeField] private PlayableAsset[] tutorials;
    [SerializeField] private GameObject[] dialogueBoxes;
    [Header("Variables that track what dialogue has pased")] 
    [SerializeField] private int dialoguePassed;
    [SerializeField] private int currentDialogue;
    [Header("Dependant Components")]
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayableDirector playDirect;
    [SerializeField] private GameObject firstDoor;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        dialoguePassed = 0;
        currentDialogue = 1;
        dialogueBoxes[1].SetActive(false);
    }
    public void IncrementDialoguePassed()
    {
       dialoguePassed++;
    }


    public void Update()
    {
        CheckDialogue();
    }

   
    public void CheckDialogue()
    {
        switch (currentDialogue)
        {
            case 1:
                dialogueBoxes[0].SetActive(true);
                playDirect.playableAsset = tutorials[0];
                playDirect.Play();
                StartCoroutine(WaitForDialogueToFinish());
                currentDialogue++;
                break;
            case 2:
                if (player.health < player.maxHealth)
                {
                    dialogueBoxes[1].SetActive(true);
                    playDirect.playableAsset = tutorials[1];
                    playDirect.Play();
                    StartCoroutine(WaitForDialogueToFinish());
                    currentDialogue++;
                    dialogueBoxes[1].SetActive(false);
                }
                break;
            case 3:
                if (player.playerPoints > 0)
                {
                    dialogueBoxes[2].SetActive(true);
                    playDirect.playableAsset = tutorials[2];
                    playDirect.Play();
                    StartCoroutine(WaitForDialogueToFinish());
                    currentDialogue++;
                    dialogueBoxes[2].SetActive(false);
                }
                break;
            case 4:
                if(firstDoor = null)
                {
                    dialogueBoxes[3].SetActive(true);
                    playDirect.playableAsset = tutorials[3];
                    playDirect.Play();
                    StartCoroutine(WaitForDialogueToFinish());
                    currentDialogue++;
                    dialogueBoxes[3].SetActive(false);
                }
                break;
            default:
                Debug.Log("No Dialogue has Played");
                break;
        }
    }

    IEnumerator WaitForDialogueToFinish()
    {
        yield return new WaitUntil(() => dialoguePassed >= currentDialogue);
    }
    
}
