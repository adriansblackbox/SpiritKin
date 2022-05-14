using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Reference Scripts")]
    [SerializeField] Enemy_Spawner es;
    [SerializeField] ScrollingText st;
    [SerializeField] PlayerController pc;

    [Header("Conditionals For World")]
    public bool tutorialFinished;
    public bool tutorialOn;
    public bool showingNonPlayerCamera;
    [SerializeField] bool shownShrine;
    [SerializeField] bool shownPool;

    [Header("Conditionals For UI")]
    [SerializeField] bool movingUIElement;
    [SerializeField] bool shownCoins;
    [SerializeField] bool shownTeas;
    [SerializeField] bool shownCurses;
    
    [Header("Cameras")]
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject shrineCamera;
    [SerializeField] GameObject poolCamera1;
    [SerializeField] GameObject poolCamera2;
    [SerializeField] GameObject poolCamera3;

    [Header("Tutorial UI Elements")]
    [SerializeField] GameObject dialogueObject;

    [Header("Sounds")]
    [SerializeField] AudioSource NPCAudio;
    [SerializeField] AudioClip heyAudio;
    [SerializeField] AudioClip[] loopingDialogueAudio;

    void Start()
    {
        if (tutorialOn)
        {
            pc.enabled = false;
            st.ActivateText();
            NPCAudio.clip = heyAudio;
            StartCoroutine("Hey");
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
            //Move onto the next NPC line or coroutine that explains next part
            else if (!tutorialFinished && !showingNonPlayerCamera && !st.typing && !movingUIElement && CheckForInput())
            {
                if (!shownShrine && st.GetCurrentDisplayingText() == 1)
                    StartCoroutine("ShowShrine");
                else if (!shownCoins && st.GetCurrentDisplayingText() == 2)
                    StartCoroutine("ShowCoins");
                else if (!shownTeas && st.GetCurrentDisplayingText() == 3)
                    StartCoroutine("ShowTeas");
                else if (!shownCurses && st.GetCurrentDisplayingText() == 4)
                    StartCoroutine("ShowCurses");
                else if (!shownPool && st.GetCurrentDisplayingText() == 5)
                    StartCoroutine("ShowPool");
                else
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

    IEnumerator Hey()
    {
        yield return new WaitForSeconds(0.5f);
        NPCAudio.Play();
    }

    IEnumerator ShowCoins()
    {
        //pop coin up in the middle of the screen
          // -> move it to its designated spot
        return null;
    }

    IEnumerator ShowTeas()
    {
        //pop up tea buff backgrounds and one at a time slot them up top
        return null;
    }

    IEnumerator ShowCurses()
    {
        //pop up curse backgrounds and one at a time slot them up top (just like with the Teas)
        return null;
    }

    IEnumerator ShowShrine()
    {
        st.ActivateText();
        shownShrine = true;
        showingNonPlayerCamera = true;
        playerCamera.GetComponent<Camera>().enabled = false;
        shrineCamera.GetComponent<Camera>().enabled = true;
        shrineCamera.GetComponent<CameraFade>().Reset();
        es.curseShrine(true); //curse the tutorial shrine
        es.spawnEnemy(es.shrineForTutorial);
        yield return new WaitForSeconds(5f);
        playerCamera.GetComponent<Camera>().enabled = true;
        playerCamera.GetComponent<CameraFade>().Reset();
        shrineCamera.GetComponent<Camera>().enabled = false;
        showingNonPlayerCamera = false;
    }

    IEnumerator ShowPool()
    {
        st.ActivateText();
        shownPool = true;
        showingNonPlayerCamera = true;
        playerCamera.GetComponent<Camera>().enabled = false;
        poolCamera1.GetComponent<Camera>().enabled = true;
        poolCamera1.GetComponent<CameraFade>().Reset();
        yield return new WaitForSeconds(3f);
        playerCamera.GetComponent<Camera>().enabled = true;
        playerCamera.GetComponent<CameraFade>().Reset();
        poolCamera1.GetComponent<Camera>().enabled = false;
        showingNonPlayerCamera = false;
    }
}
