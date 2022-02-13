using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static blindCurse;
using static invertCurse;
using static slowCurse;

public class CurseMeter : MonoBehaviour
{
    private CharacterStats pStats;
    private int soulDelta;

    public bool newCurse;
    public float curseMeter;
    public float fillRate = 10.0f;
    public List<Curse> curseArray = new List<Curse>();
    public List<Curse> activeCurses = new List<Curse>();
    //public GameObject[] cursesUI;
    //private GameObject curCurseUI;
    //public image empty_notch, slow, blind, etc....

    // Start is called before the first frame update
    void Start()
    {
        newCurse = false;
        curseMeter = 0f;
        pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
        soulDelta = pStats.currSouls;

        blindCurse blind = new blindCurse();
        slowCurse slow = new slowCurse();
        invertCurse invert = new invertCurse();

        curseArray.Add(blind);
        curseArray.Add(slow);
        curseArray.Add(invert);

        Debug.Log("Array: " + curseArray);
        //cursesUI[1].bar.enabled = cursesUI[2].bar.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(pStats.currSouls < soulDelta)
        {
            soulDelta = pStats.currSouls;
        }

        if(activeCurses.Count < 3)
        {
            if(soulDelta < pStats.currSouls) 
            {
                if(soulDelta == 0)
                {
                    curseMeter += (((float)pStats.currSouls - soulDelta) / (float)pStats.currSouls) / fillRate;
                }
                else
                {
                    curseMeter += ((float)pStats.currSouls - (float)soulDelta) / fillRate;
                }
                
                soulDelta = pStats.currSouls;
            }

            if(curseMeter >= 1f)
            {
                curseMeter = 0;
                addCurse();
            }
        }

        if(newCurse)
        {
            CurseHandler();
            newCurse = false;
        }
        // manageCurseUI();
    }

    public void addCurse()
    {
        List<Curse> unactiveCurses = curseArray.Except(activeCurses).ToList();
        if(unactiveCurses.Count > 0)
        {
            unactiveCurses[Random.Range(0, curseArray.Count - 1)].active = true;
        }
        newCurse = true;
    }
    /*
    private void manageCurseUI(){
        currentCurseUI = cursesUI[activeCurses.Count];
        currentCurseUI.getChild(0).bar.enabled = true;
        currentCurseUI.getChild(0).fill = curseMeter;

    }
    */

    public void removeCurse() 
    {
        if(activeCurses.Count == 0)
        {
            Debug.Log("No curses to clear!");
            return;
        }
        int i = Random.Range(0, curseArray.Count - 1);
        
        activeCurses[i].active = true;
        activeCurses[i].isApplied = false;
        activeCurses[i].removeFlag = true;
        activeCurses.RemoveAt(i);
        /*
            switch(i){
                case 0:
                    activeCurse[1].image = activeCurse[0].image
                case 1;
                    activeCurse[2].image = activeCurse[1].image
                case 2:
                    ativeCurse[2].image = empty_notch;
            }
        */
        newCurse = true;
    }

    public void CurseHandler () 
    {
        curseArray.ForEach(x =>
            {
                Debug.Log(x);
                if(x.active && !x.isApplied)
                {
                    /*
                    currentCurse.getChild(0).image.enabled = true;
                    currentCurse.getChild(0).image = findImage(x.type);
                    currentCurse.getChild(1).bar.enabled = false;
                    */
                    x.invokeCurse();
                }
                if(x.removeFlag) x.removeCurse();
            } 
        );
    }
}
