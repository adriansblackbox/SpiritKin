using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] [TextArea] private string[] dialogueLines;
    [SerializeField] private float textSpeed = 0.03f;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    private int currentDisplayingText = 0;
    [SerializeField] private GameObject nextLineIndicator;

    [Header("Player Control")]
    public PlayerController playerController;

    [Header("Debugging")]
    [SerializeField] private bool typing;
    [SerializeField] private bool finishedWithTutorial;
    private float myTime;

    private void Start()
    {
        playerController.enabled = false;
        StartCoroutine("AnimateText");
    }

    private void Update()
    {
        if (currentDisplayingText == dialogueLines.Length && !typing) //tutorial is finished
        {
            finishedWithTutorial = true;
        }

        //flicker arrow
        if (!typing)
        {
            myTime += Time.deltaTime;
            if (myTime > 0.5f)
            {
                myTime = 0;
                nextLineIndicator.SetActive(!nextLineIndicator.activeSelf);
            }
        }
        else //hide arrow
        {
            nextLineIndicator.SetActive(false);
        }


        //probably needs to be done in a more robust manner
        if (finishedWithTutorial && (Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Space)) )
        {
            playerController.enabled = true;
            gameObject.SetActive(false);
        }
        else if (typing && (Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Space)) )
        {
            DeactivateText();
        }
        else if (!typing && (Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Space)) )
        {
            if (currentDisplayingText < dialogueLines.Length)
            {
                typing = true;
                StartCoroutine("AnimateText");
            }
        }
    }

    private void DeactivateText()
    {
        Debug.Log("Deactivating Text");
        StopCoroutine("AnimateText");
        typing = false;
        dialogueText.text = dialogueLines[currentDisplayingText];
        currentDisplayingText++;
    }

    IEnumerator AnimateText()
    {
        typing = true;
        for (int i = 0; i < dialogueLines[currentDisplayingText].Length + 1; i++)
        {
            dialogueText.text = dialogueLines[currentDisplayingText].Substring(0,i);
            yield return new WaitForSeconds(textSpeed);
        }
        typing = false;
        currentDisplayingText++;
        yield return null;
    }
}
