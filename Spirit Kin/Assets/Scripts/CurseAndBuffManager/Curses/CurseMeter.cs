using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject[] cursesUI;
    private GameObject curCurseUI;
    public Image Notch;

    // Start is called before the first frame update
    void Start()
    {
        newCurse = false;
        curseMeter = 0f;
        pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
        soulDelta = pStats.currSouls;

        damageCurse weak = new damageCurse();
        slowCurse slow = new slowCurse();
        armorCurse frail = new armorCurse();

        curseArray.Add(weak);
        curseArray.Add(slow);
        curseArray.Add(frail);

        Debug.Log("Array: " + curseArray);
        cursesUI[1].transform.Find("Bar").gameObject.active = cursesUI[2].transform.Find("Bar").gameObject.active = false;
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
    
    private void manageCurseUI(){
        curCurseUI = cursesUI[activeCurses.Count];
        curCurseUI.transform.Find("Bar").gameObject.active = true;
        curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = curseMeter;

    }
   

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

        switch(i){
            case 0:
                cursesUI[0].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite;
                goto case 1;
            case 1:
                cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = cursesUI[2].transform.Find("Curse").gameObject.GetComponent<Image>().sprite;
                goto case 2;
            case 2:
                cursesUI[2].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = Notch.sprite; // Hide curse with an invisible circle
                break;
        }
    
        newCurse = true;
    }

    public void CurseHandler () 
    {
        curseArray.ForEach(x =>
            {
                Debug.Log(x);
                if(x.active && !x.isApplied)
                {
                    curCurseUI.transform.Find("Curse").gameObject.active = true;
                    curCurseUI.transform.Find("Curse").gameObject.GetComponent<Image>().sprite = x.image.sprite;
                    curCurseUI.transform.Find("Bar").gameObject.active = false;
                    x.invokeCurse();
                }
                if(x.removeFlag) x.removeCurse();
            } 
        );
    }
}
