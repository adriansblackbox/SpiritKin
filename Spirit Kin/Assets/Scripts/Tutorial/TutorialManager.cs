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
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject shrineCamera;
    [SerializeField] GameObject poolCamera1;
    [SerializeField] GameObject poolCamera2;
    [SerializeField] GameObject poolCamera3;

    [Header("Tutorial UI Elements")]
    [SerializeField] GameObject dialogueObject;


    void Start()
    {
        if (tutorialOn)
        {
            pc.enabled = false;
            st.ActivateText();
        }
        else
        {
            dialogueObject.SetActive(false);
        }
    }

    void Update()
    {
        if (tutorialOn)
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
    }

    private bool CheckForInput()
    {
        if (Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
            return true;
        else
            return false;
    }

    IEnumerator ShowShrine()
    {
        shownShrine = true;
        showingNonPlayerCamera = true;
        playerCamera.GetComponent<Camera>().enabled = false;
        shrineCamera.GetComponent<Camera>().enabled = true;
        shrineCamera.GetComponent<CameraFade>().Reset();
        es.curseShrine(true); //curse the tutorial shrine
        es.spawnEnemy(es.shrineForTutorial);
        st.ActivateText();
        yield return new WaitForSeconds(5f);
        playerCamera.GetComponent<Camera>().enabled = true;
        playerCamera.GetComponent<CameraFade>().Reset();
        shrineCamera.GetComponent<Camera>().enabled = false;
        showingNonPlayerCamera = false;
    }

    IEnumerator ShowPool()
    {
        shownPool = true;
        showingNonPlayerCamera = true;
        playerCamera.GetComponent<Camera>().enabled = false;
        poolCamera1.GetComponent<Camera>().enabled = true;
        poolCamera1.GetComponent<CameraFade>().Reset();
        st.ActivateText();
        yield return new WaitForSeconds(3f);
        playerCamera.GetComponent<Camera>().enabled = true;
        playerCamera.GetComponent<CameraFade>().Reset();
        poolCamera1.GetComponent<Camera>().enabled = false;
        showingNonPlayerCamera = false;
    }
}
