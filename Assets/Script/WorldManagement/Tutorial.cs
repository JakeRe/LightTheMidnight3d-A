using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Tutorial : MonoBehaviour
{
    [Header("Arrays for Tutorial Timelines and Dialogue")]
    [SerializeField] private PlayableAsset[] tutorials;
    [SerializeField] public GameObject[] dialogueBoxes;
    [Header("Variables that track what dialogue has pased")] 
    [SerializeField] private int dialoguePassed;
    [SerializeField] public int currentDialogue;
    [Header("Dependant Components")]
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayableDirector playDirect;
    [SerializeField] private GameObject firstDoor;
    [SerializeField] private GameObject activeDialogueBox;
    [SerializeField] private bool foodTutorialComplete;
    [Header("Game Manager")]
    [SerializeField] private GameManager gameManage;
    [SerializeField] private WaveSystem waveManager;

    private void Start()
    {
        gameManage = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
        dialoguePassed = 0;
        currentDialogue = 1;
        activeDialogueBox = dialogueBoxes[0];
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
        SkipTutorial();
    }

   
    public void CheckDialogue()
    {
        if (!gameManage.isPaused)
        {
            playDirect.Resume();

            switch (currentDialogue)
            {
                case 1:
                    playDirect.playableAsset = tutorials[0];
                    dialogueBoxes[0] = activeDialogueBox;
                    dialogueBoxes[0].SetActive(true);
                    playDirect.Play();
                    currentDialogue += 1;
                    StartCoroutine(WaitForDialogueToFinish());
                    dialogueBoxes[0].SetActive(false);
                    break;
                case 2:
                    if (player.health < player.maxHealth && dialogueBoxes[0].activeInHierarchy == false)
                    {
                        activeDialogueBox = dialogueBoxes[1];
                        playDirect.playableAsset = tutorials[1];
                        dialogueBoxes[1].SetActive(true);
                        playDirect.Play();
                        currentDialogue += 1;

                    }
                    else if (player.health == player.maxHealth && dialogueBoxes[0].activeInHierarchy == false)
                    {
                        StartCoroutine(TimeSkip());
                        playDirect.playableAsset = tutorials[1];
                        dialogueBoxes[0].SetActive(false);
                        activeDialogueBox = dialogueBoxes[1];
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
                        activeDialogueBox = dialogueBoxes[2];
                        dialogueBoxes[2].SetActive(true);
                        playDirect.Play();
                        dialogueBoxes[2].SetActive(false);
                        currentDialogue += 1;
                    }
                    StartCoroutine(WaitForDialogueToFinish());
                    break;
                case 4:
                    if (firstDoor == null)
                    {
                        playDirect.playableAsset = tutorials[3];
                        activeDialogueBox = dialogueBoxes[3];
                        dialogueBoxes[3].SetActive(true);
                        playDirect.Play();
                        dialogueBoxes[3].SetActive(false);
                        currentDialogue += 1;
                    }
                    StartCoroutine(WaitForDialogueToFinish());
                    break;
                default:
                    break;
            }
        }
        else if(gameManage.isPaused)
        {
            playDirect.Pause();
        }
       
       
    }

    void SkipTutorial()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentDialogue = 4;
            dialoguePassed = 3;
            playDirect.Stop();
            activeDialogueBox.SetActive(false);
            waveManager.gameObject.SetActive(true);

        }
    }

    IEnumerator WaitForDialogueToFinish()
    {
        yield return new WaitUntil(() => dialoguePassed == currentDialogue);
    }
    
    IEnumerator TimeSkip()
    {
        yield return new WaitForSecondsRealtime(10);
    }
}
