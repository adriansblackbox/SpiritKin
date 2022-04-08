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

    public bool newCurse;
    public float curseMeter;
    public float fillRate; // Value is inverse to rate
    public List<Curse> curseArray = new List<Curse>();
    public List<Curse> activeCurses = new List<Curse>();
    public GameObject[] cursesUI;
    public GameObject Sword0, Sword1, Sword2, Sword3;
    private GameObject curCurseUI;
    public Sprite Notch, weakImage, slowImage, frailImage;
    public GameObject ActiveSword;

    public bool debugbool = false;

    // Start is called before the first frame update
    private void Start()
    {
        newCurse = false;
        curseMeter = 0f;
        pStats = gameObject.GetComponent<PlayerStats>();

        damageCurse weak = new damageCurse(weakImage, gameObject.GetComponent<PlayerStats>(), this);
        slowCurse slow = new slowCurse(slowImage, gameObject.GetComponent<PlayerStats>(), this);
        armorCurse frail = new armorCurse(frailImage, gameObject.GetComponent<PlayerStats>(), this);

        curseArray.Add(weak);
        curseArray.Add(slow);
        curseArray.Add(frail);

        cursesUI[1].transform.Find("Bar").gameObject.SetActive(false);
        cursesUI[2].transform.Find("Bar").gameObject.SetActive(false);
        curCurseUI = cursesUI[0];
        curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = curseMeter;
        ActiveSword = Sword0;
        Sword1.SetActive(false); Sword2.SetActive(false); Sword3.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (activeCurses.Count < 3)
        {
            curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = curseMeter;
            if (curseMeter >= 1f)
            {
                curseMeter = 0;
                curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = curseMeter;
                addCurse();
            }
        }

        if (debugbool)
        {
            removeCurse();
            //debugbool = !debugbool;
        }

        if (newCurse)
        {
            CurseHandler();
            newCurse = false;
        }
        if(!FindObjectOfType<PlayerCombat>().isDodging)
            HandleSword();

    }
    private void HandleSword()
    {
        switch (activeCurses.Count)
        {
            case 0:
                Sword0.SetActive(true);
                Sword1.SetActive(false);
                Sword2.SetActive(false);
                Sword3.SetActive(false);
                if(ActiveSword != Sword0)
                    ActiveSword.GetComponent<SwordCollision>().DisableHitRay();
                ActiveSword = Sword0;
                break;
            case 1:
                Sword1.SetActive(true);
                Sword0.SetActive(false);
                Sword2.SetActive(false);
                Sword3.SetActive(false);
                if(ActiveSword != Sword1)
                    ActiveSword.GetComponent<SwordCollision>().DisableHitRay();
                ActiveSword = Sword1;
                break;
            case 2:
                Sword2.SetActive(true);
                Sword1.SetActive(false);
                Sword0.SetActive(false);
                Sword3.SetActive(false);
                if(ActiveSword != Sword2)
                    ActiveSword.GetComponent<SwordCollision>().DisableHitRay();
                ActiveSword = Sword2;
                break;
            case 3:
                Sword3.SetActive(true);
                Sword1.SetActive(false);
                Sword2.SetActive(false);
                Sword0.SetActive(false);
                if(ActiveSword != Sword3)
                    ActiveSword.GetComponent<SwordCollision>().DisableHitRay();
                ActiveSword = Sword3;
                break;
        }
    }

    public void addCurse()
    {
        List<Curse> unactiveCurses = curseArray.Except(activeCurses).ToList();
        if (unactiveCurses.Count > 0)
        {
            unactiveCurses[Random.Range(0, unactiveCurses.Count - 1)].active = true;
        }
        newCurse = true;
    }

    private void manageCurseUI()
    {
        switch (activeCurses.Count)
        {
            case 3:
                cursesUI[0].transform.Find("Bar").gameObject.SetActive(false);
                cursesUI[1].transform.Find("Bar").gameObject.SetActive(false);
                cursesUI[2].transform.Find("Bar").gameObject.SetActive(false);
                break; // All curses active. Flow for managing player max health.
            case 2:
                cursesUI[2].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = Notch;
                cursesUI[0].transform.Find("Bar").gameObject.SetActive(false);
                cursesUI[1].transform.Find("Bar").gameObject.SetActive(false);
                cursesUI[2].transform.Find("Bar").gameObject.SetActive(true);
                break;
            case 1:
                cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = Notch;
                cursesUI[0].transform.Find("Bar").gameObject.SetActive(false);
                cursesUI[1].transform.Find("Bar").gameObject.SetActive(true);
                cursesUI[2].transform.Find("Bar").gameObject.SetActive(false);
                break;
            case 0:
                cursesUI[0].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = Notch;
                cursesUI[0].transform.Find("Bar").gameObject.SetActive(true);
                cursesUI[1].transform.Find("Bar").gameObject.SetActive(false);
                cursesUI[2].transform.Find("Bar").gameObject.SetActive(false);
                break;
        }

        if (activeCurses.Count < 3)
        {
            curCurseUI = cursesUI[activeCurses.Count];
        }
    }


    public void removeCurse()
    {
        if (activeCurses.Count == 0)
        {
            Debug.Log("No curses to clear!");
            return;
        }
        int i = Random.Range(0, activeCurses.Count - 1);

        FindObjectOfType<StatVFX>().removeCurseStat(activeCurses[i].type);

        activeCurses[i].active = true;
        activeCurses[i].isApplied = false;
        activeCurses[i].removeFlag = true;
        activeCurses.RemoveAt(i);
        cursesUI[i].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = Notch; // Hide curse with an invisible circle

        switch (i)
        {
            case 0:
                cursesUI[0].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite;
                goto case 1;
            case 1:
                cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = cursesUI[2].transform.Find("Curse").gameObject.GetComponent<Image>().sprite;
                break;
        }

        newCurse = true;
    }

    public void CurseHandler()
    {
        curseArray.ForEach(x =>
            {
                if (x.removeFlag)
                {
                    x.removeCurse();
                    manageCurseUI();
                }
                else if (x.active && !x.isApplied)
                {
                    manageCurseUI();
                    curCurseUI.transform.Find("Curse").gameObject.SetActive(true);
                    var a = x.image;
                    curCurseUI.transform.Find("Curse").gameObject.GetComponent<Image>().sprite = a;
                    curCurseUI.transform.Find("Bar").gameObject.SetActive(false);
                    x.invokeCurse();
                    manageCurseUI();
                    FindObjectOfType<StatVFX>().addCurseStat(x.type);
                }

            }
        );
    }
}
