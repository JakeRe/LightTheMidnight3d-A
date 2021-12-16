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
    [SerializeField] private GameObject activeDialogueBox;
    [SerializeField] private bool foodTutorialComplete;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        dialoguePassed = 0;
        currentDialogue = 1;
        dialogueBoxes[1].SetActive(false);
        dialogueBoxes[0].SetActive(true);
    }
    public void IncrementDialoguePassed()
    {
        dialoguePassed+=1;
    }

    public void FoodTutorialComplete()
    {
        foodTutorialComplete = true;
    }

    public void Update()
    {
        CheckDialogue();
        Debug.Log($"Current Dialogue is {currentDialogue}");
    }

   
    public void CheckDialogue()
    {
        switch (currentDialogue)
        {
            case 1:
                playDirect.playableAsset = tutorials[0];
                dialogueBoxes[0].SetActive(true);
                playDirect.Play();
                currentDialogue += 1;
                StartCoroutine(WaitForDialogueToFinish());
                break;
            case 2:
                if (player.health < player.maxHealth && dialogueBoxes[0].activeInHierarchy == false)
                {
                    playDirect.playableAsset = tutorials[1];
                    dialogueBoxes[1].SetActive(true);
                    playDirect.Play();
                    //dialogueBoxes[1].SetActive(false);
                    currentDialogue += 1;

                }
                StartCoroutine(WaitForDialogueToFinish());
                break;
            case 3:
                if (player.playerPoints > 500 && foodTutorialComplete)
                {
                    playDirect.playableAsset = tutorials[2];
                    dialogueBoxes[2].SetActive(true);
                    playDirect.Play();
                    dialogueBoxes[2].SetActive(false);
                    currentDialogue += 1;
                }
                StartCoroutine(WaitForDialogueToFinish());
                break;
            case 4:
                if(firstDoor == null)
                {
                    Debug.Log("Initiate 4th Dialogue");
                    playDirect.playableAsset = tutorials[3];
                    dialogueBoxes[3].SetActive(true);
                    playDirect.Play();
                    dialogueBoxes[3].SetActive(false);
                    currentDialogue += 1;
                }
                StartCoroutine(WaitForDialogueToFinish());
                break;
            default:
                Debug.Log("No Dialogue has Played");
                break;
        }
    }

    IEnumerator WaitForDialogueToFinish()
    {
        Debug.Log("Coroutine Entered");
        yield return new WaitUntil(() => dialoguePassed == currentDialogue);
    }
    
}
