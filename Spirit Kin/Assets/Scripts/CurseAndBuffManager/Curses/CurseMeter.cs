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
    public Sprite Notch, weakImage, slowImage, frailImage;

    public bool debugbool = false;

    // Start is called before the first frame update
    void Start()
    {
        newCurse = false;
        curseMeter = 0f;
        pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
        soulDelta = pStats.currSouls;

        damageCurse weak = new damageCurse(weakImage, pStats);
        slowCurse slow = new slowCurse(slowImage, pStats);
        armorCurse frail = new armorCurse(frailImage, pStats);

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

        if(debugbool){
            removeCurse();
            //debugbool = !debugbool;
        }

        if(newCurse)
        {
            manageCurseUI();
            CurseHandler();
            newCurse = false;
        }
        
    }

    public void addCurse()
    {
        List<Curse> unactiveCurses = curseArray.Except(activeCurses).ToList();
        if(unactiveCurses.Count > 0)
        {
            unactiveCurses[Random.Range(0, unactiveCurses.Count - 1)].active = true;
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
        cursesUI[i].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = Notch; // Hide curse with an invisible circle

        switch(i){
            case 0:
                cursesUI[0].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite;
                goto case 1;
            case 1:
                cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = cursesUI[2].transform.Find("Curse").gameObject.GetComponent<Image>().sprite;
                break;
        }
        
        newCurse = true;
    }

    public void CurseHandler () 
    {
        curseArray.ForEach(x =>
            {
                if(x.removeFlag) x.removeCurse();
                else if(x.active && !x.isApplied)
                {
                    curCurseUI.transform.Find("Curse").gameObject.active = true;
                    var a = x.image;
                    curCurseUI.transform.Find("Curse").gameObject.GetComponent<Image>().sprite = a;
                    curCurseUI.transform.Find("Bar").gameObject.active = false;
                    x.invokeCurse();
                }
                
            } 
        );
    }
}
