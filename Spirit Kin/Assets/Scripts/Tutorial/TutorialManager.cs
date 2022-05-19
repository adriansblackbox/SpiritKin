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

    [SerializeField] float moveTime;
    public float durationOfMove;

    private Vector2 startPositionForCoin;
    private Vector2 startPositionForBuffs;
    private Vector2 startPositionForCurses;

    [SerializeField] GameObject coinObject;
    public Vector2 coinIntendedPosition;
    
    [SerializeField] GameObject firstBuff;
    public Vector2 firstBuffIntendedPosition;
    [SerializeField] GameObject secondBuff;
    public Vector2 secondBuffIntendedPosition;
    [SerializeField] GameObject thirdBuff;
    public Vector2 thirdBuffIntendedPosition;
    
    [SerializeField] GameObject firstCurse;
    public Vector2 firstCurseIntendedPosition;
    [SerializeField] GameObject secondCurse;
    public Vector2 secondCurseIntendedPosition;
    [SerializeField] GameObject thirdCurse;
    public Vector2 thirdCurseIntendedPosition;

    [Header("Sounds")]
    [SerializeField] AudioSource NPCAudio;
    [SerializeField] AudioClip heyAudio;
    [SerializeField] AudioClip[] loopingDialogueAudio;

    void Start()
    {
        if (tutorialOn)
        {
            calculateMiddlePoints();
            pc.enabled = false;
            st.ActivateText();
            StartCoroutine("Hey");
        }
        else
        {
            dialogueObject.SetActive(false);
            coinObject.SetActive(true);
            firstBuff.SetActive(true);
            secondBuff.SetActive(true);
            thirdBuff.SetActive(true);
            firstCurse.SetActive(true);
            secondCurse.SetActive(true);
            thirdCurse.SetActive(true);
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
                st.ActivateText();
                chooseAndPlayDialogueSound();
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
            }
        }
    }

    private void calculateMiddlePoints()
    {
        float canvasX = coinObject.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        float canvasY = coinObject.transform.parent.GetComponent<RectTransform>().anchoredPosition.y;

        //coins 0,0 is top left so I need to get there and then do canvasX / 2 canvasY / 2

        float coinX = coinObject.GetComponent<RectTransform>().anchoredPosition.x;
        float coinY = coinObject.GetComponent<RectTransform>().anchoredPosition.y;
        
        startPositionForCoin = new Vector2(canvasX/2, -canvasY / 2 + coinY * 1.5f);

        //Get X value for one of the containers (will be used as negative for buffs & positive for curses)
        float buffAndCurseContainerX = firstBuff.transform.parent.GetComponent<RectTransform>().anchoredPosition.x;
        //get 1/4 of the icon size so its centered (not 1/2 because the icons are circular and only take up about half of the overarching container)
        float iconSize = firstBuff.GetComponent<RectTransform>().sizeDelta.x / 4;

        startPositionForBuffs = new Vector2(-buffAndCurseContainerX - iconSize, -canvasY / 2);
        startPositionForCurses = new Vector2(buffAndCurseContainerX + iconSize, -canvasY / 2);
    }

    private bool CheckForInput()
    {
        if (Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
            return true;
        else
            return false;
    }

    private void chooseAndPlayDialogueSound()
    {
        int rand = Random.Range(0, loopingDialogueAudio.Length);
        NPCAudio.clip = loopingDialogueAudio[rand];
        NPCAudio.Play();
    }

    IEnumerator Hey()
    {
        yield return new WaitForSeconds(0.5f);
        NPCAudio.clip = heyAudio;
        NPCAudio.Play();
    }

    IEnumerator ShowCoins()
    {
        //would be really cool to have a bunch of coins go into the top left like lego star wars bits and increment the money amount

        movingUIElement = true;
        coinObject.SetActive(true);
        coinObject.transform.GetChild(1).gameObject.SetActive(false);
        coinObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(startPositionForCoin.x, startPositionForCoin.y, 0f);

        yield return new WaitForSeconds(0.25f);

        while (moveTime < durationOfMove + 0.5f)
        {
            moveTime += Time.deltaTime;
            coinObject.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPositionForCoin, new Vector3(coinIntendedPosition.x, coinIntendedPosition.y, 0f), moveTime/(durationOfMove + 0.5f));
            yield return new WaitForSeconds(0.001f);
        }
        moveTime = 0f;
        coinObject.transform.GetChild(1).gameObject.SetActive(true);
        movingUIElement = false;
    }

    IEnumerator ShowTeas()
    {
        movingUIElement = true;
        
        firstBuff.SetActive(true);
        firstBuff.GetComponent<RectTransform>().anchoredPosition = new Vector3(startPositionForBuffs.x, startPositionForBuffs.y, 0f);
        yield return new WaitForSeconds(0.25f);

        while (moveTime < durationOfMove)
        {
            moveTime += Time.deltaTime;
            firstBuff.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPositionForBuffs, new Vector3(firstBuffIntendedPosition.x, firstBuffIntendedPosition.y, 0f), moveTime/durationOfMove);
            yield return new WaitForSeconds(0.001f);
        }
        moveTime = 0f;
        
        secondBuff.SetActive(true);
        secondBuff.GetComponent<RectTransform>().anchoredPosition = new Vector3(startPositionForBuffs.x, startPositionForBuffs.y, 0f);
        yield return new WaitForSeconds(0.25f);

        while (moveTime < durationOfMove)
        {
            moveTime += Time.deltaTime;
            secondBuff.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPositionForBuffs, new Vector3(secondBuffIntendedPosition.x, secondBuffIntendedPosition.y, 0f), moveTime/durationOfMove);
            yield return new WaitForSeconds(0.001f);
        }
        moveTime = 0f;

        thirdBuff.SetActive(true);
        thirdBuff.GetComponent<RectTransform>().anchoredPosition = new Vector3(startPositionForBuffs.x, startPositionForBuffs.y, 0f);
        yield return new WaitForSeconds(0.25f);

        while (moveTime < durationOfMove)
        {
            moveTime += Time.deltaTime;
            thirdBuff.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPositionForBuffs, new Vector3(thirdBuffIntendedPosition.x, thirdBuffIntendedPosition.y, 0f), moveTime/durationOfMove);
            yield return new WaitForSeconds(0.001f);
        }
        moveTime = 0f;
        
        movingUIElement = false;
    }

    IEnumerator ShowCurses()
    {
        movingUIElement = true;

        firstCurse.SetActive(true);
        firstCurse.GetComponent<RectTransform>().anchoredPosition = new Vector3(startPositionForCurses.x, startPositionForCurses.y, 0f);
        yield return new WaitForSeconds(0.25f);

        while (moveTime < durationOfMove)
        {
            moveTime += Time.deltaTime;
            firstCurse.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPositionForCurses, new Vector3(firstCurseIntendedPosition.x, firstCurseIntendedPosition.y, 0f), moveTime/durationOfMove);
            yield return new WaitForSeconds(0.001f);
        }
        moveTime = 0f;
        
        secondCurse.SetActive(true);
        secondCurse.GetComponent<RectTransform>().anchoredPosition = new Vector3(startPositionForCurses.x, startPositionForCurses.y, 0f);
        yield return new WaitForSeconds(0.25f);

        while (moveTime < durationOfMove)
        {
            moveTime += Time.deltaTime;
            secondCurse.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPositionForCurses, new Vector3(secondCurseIntendedPosition.x, secondCurseIntendedPosition.y, 0f), moveTime/durationOfMove);
            yield return new WaitForSeconds(0.001f);
        }
        moveTime = 0f;

        thirdCurse.SetActive(true);
        thirdCurse.GetComponent<RectTransform>().anchoredPosition = new Vector3(startPositionForCurses.x, startPositionForCurses.y, 0f);
        yield return new WaitForSeconds(0.25f);

        while (moveTime < durationOfMove)
        {
            moveTime += Time.deltaTime;
            thirdCurse.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPositionForCurses, new Vector3(thirdCurseIntendedPosition.x, thirdCurseIntendedPosition.y, 0f), moveTime/durationOfMove);
            yield return new WaitForSeconds(0.001f);
        }
        moveTime = 0f;

        movingUIElement = false;
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

        yield return new WaitForSeconds(3f);
        poolCamera2.GetComponent<Camera>().enabled = true;
        poolCamera1.GetComponent<Camera>().enabled = false;

        yield return new WaitForSeconds(3f);
        poolCamera3.GetComponent<Camera>().enabled = true;
        poolCamera2.GetComponent<Camera>().enabled = false;

        yield return new WaitForSeconds(3f);
        playerCamera.GetComponent<Camera>().enabled = true;
        poolCamera3.GetComponent<Camera>().enabled = false;
        showingNonPlayerCamera = false;
    }
}
