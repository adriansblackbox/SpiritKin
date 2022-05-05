using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Reference Scripts")]
    [SerializeField] Enemy_Spawner es;
    [SerializeField] ScrollingText st;
    [SerializeField] PlayerController pc;

    [Header("Conditionals")]
    public bool tutorialFinished;
    public bool tutorialOn;
    public bool showingNonPlayerCamera;
    [SerializeField] bool shownShrine;
    [SerializeField] bool shownPool;
    
    [Header("Cameras")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera shrineCamera;
    [SerializeField] Camera poolCamera1;
    [SerializeField] Camera poolCamera2;
    [SerializeField] Camera poolCamera3;

    [Header("Tutorial UI Elements")]
    [SerializeField] GameObject dialogueObject;

    void Start()
    {
        pc.enabled = false;
        st.ActivateText();
    }

    void Update()
    {
        //show shrine
        if (!shownShrine && st.GetCurrentDisplayingText() == 1 && CheckForInput())
        {
            StartCoroutine("ShowShrine");
        }

        //show purification pool
        if (!shownPool && st.GetCurrentDisplayingText() == 4 && CheckForInput())
        {
            StartCoroutine("ShowPool");
        }

        //All dialogue has been shown and they give input
        if (st.CheckIfDialogueCompleted() && CheckForInput())
        {
            tutorialFinished = true;
            dialogueObject.SetActive(false);
            pc.enabled = true;
        }
        //Stop typing and show entirety of NPC's line
        else if (!tutorialFinished && !showingNonPlayerCamera && st.typing && CheckForInput())
        {
            st.DeactivateText();
        }
        //Move onto the next NPC line
        else if (!tutorialFinished && !showingNonPlayerCamera && !st.typing && CheckForInput())
        {
            st.ActivateText();
        }
    }

    private bool CheckForInput()
    {
        if (Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Space))
            return true;
        else
            return false;
    }

    IEnumerator ShowShrine()
    {
        shownShrine = true;
        showingNonPlayerCamera = true;
        playerCamera.enabled = false;
        shrineCamera.enabled = true;
        es.curseShrine(true); //curse the tutorial shrine
        es.spawnEnemy(es.shrineForTutorial);
        st.ActivateText();
        yield return new WaitForSeconds(8f);
        playerCamera.enabled = true;
        shrineCamera.enabled = false;
        showingNonPlayerCamera = false;
    }

    IEnumerator ShowPool()
    {
        shownPool = true;
        showingNonPlayerCamera = true;
        playerCamera.enabled = false;
        poolCamera1.enabled = true;
        st.ActivateText();
        yield return new WaitForSeconds(5f);
        playerCamera.enabled = true;
        poolCamera1.enabled = false;
        showingNonPlayerCamera = false;
    }
}
