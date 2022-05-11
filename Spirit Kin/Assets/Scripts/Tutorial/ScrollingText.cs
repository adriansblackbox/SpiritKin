using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    [SerializeField] TutorialManager tm;

    [Header("Text Settings")]
    [SerializeField] [TextArea] string[] dialogueLines;
    [SerializeField] float textSpeed = 0.02f;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI dialogueText;
    private int currentDisplayingText = 0;
    [SerializeField] GameObject nextLineIndicator;

    [Header("Debugging")]
    public bool typing;
    
    private float myTime;

    private void Update()
    {
        //flicker arrow
        if (!typing && !tm.showingNonPlayerCamera)
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
    }

    public bool CheckIfDialogueCompleted()
    {
        if (currentDisplayingText == dialogueLines.Length) 
            return true;
        else 
            return false;
    }

    public int GetCurrentDisplayingText() { return currentDisplayingText; }

    public void ActivateText()
    {
        Debug.Log("Activating Text");
        StartCoroutine("AnimateText");
    }

    public void DeactivateText()
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
    }
}
