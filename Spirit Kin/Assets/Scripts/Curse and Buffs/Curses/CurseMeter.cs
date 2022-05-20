using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static blindCurse;
using static invertCurse;
using static slowCurse;
using static blindCurse;
using static invertCurse;

public class CurseMeter : MonoBehaviour
{
    private PlayerStats pStats;

    public bool newCurse;

    public float curseMeter;
    public float fillRate; // Value is inverse to rate
    public List<Curse> curseArray = new List<Curse>();
    public List<Curse> activeCurses = new List<Curse>();
    public GameObject[] cursesUI;
    private GameObject curCurseUI;
    public Sprite Notch, weakImage, slowImage, frailImage, blindImage, invertImage, moneyImage, vampImage, teaImage, rangeImage;
    public GameObject ActiveSword;
    public GameObject[] shrines;
    public GameObject blindVignette;
    public GameObject healthBar;
    public SwordCollision swordLength;
    public TeaShopManager teaList;

    // Curses that need to talk to other scripts and functions easier
    public moneyCurse kromer;
    public rangeBlessing swordRangeBlessing;

    // Needed Curse Obtained Popup
    public GameObject cursePopup;
    private CursePopup cp;

    public bool deathWipe = false;

    // Start is called before the first frame update
    private void Start() {
        newCurse = false;
        curseMeter = 0f;
        pStats = gameObject.GetComponent<PlayerStats>();

        damageCurse weak = new damageCurse(weakImage, pStats, this);
        slowCurse slow = new slowCurse(slowImage, pStats, this);
        armorCurse frail = new armorCurse(frailImage, pStats, this);
        blindCurse blind = new blindCurse(blindImage, this, shrines, blindVignette, healthBar);
        invertCurse invert = new invertCurse(invertImage, gameObject);
        kromer = new moneyCurse(moneyImage, pStats, this);
        swordRangeBlessing = new rangeBlessing(rangeImage, swordLength, gameObject, this);
        teaBlessing tea = new teaBlessing(teaImage, teaList, pStats, this);
        vampBlessing vamp = new vampBlessing(vampImage, swordLength, gameObject, this);

        curseArray.Add(weak);
        curseArray.Add(slow);
        curseArray.Add(frail);
        curseArray.Add(blind);
        curseArray.Add(invert);
        curseArray.Add(kromer);
        curseArray.Add(swordRangeBlessing);
        curseArray.Add(tea);
        curseArray.Add(vamp);

        curCurseUI = cursesUI[0];
        curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = curseMeter;

        cp = cursePopup.GetComponent<CursePopup>();
    }

    // Update is called once per frame
    private void Update() {
        if (activeCurses.Count < 3) {
            curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = curseMeter;
            if (curseMeter >= 1f) {
                addCurse();
            }
        }

        if (deathWipe) { // Debug to remove all curses
            removeCurse();
        }

        if (newCurse) { // If this is flipped, we need to update something about curses
            CurseHandler();
            newCurse = false;
        }
    }


    public void addCurse() {
        List<Curse> unactiveCurses = curseArray.Except(activeCurses).ToList();
        if (unactiveCurses.Count > 0) {
            unactiveCurses[Random.Range(0, unactiveCurses.Count)].active = true;
        }
        curseMeter = 0;
        newCurse = true;
    }

    private void manageCurseUI() {
        switch (activeCurses.Count) {
            case 3:
                cursesUI[2].transform.Find("Curse").gameObject.SetActive(true);
                cursesUI[2].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 1;
                break; // All curses active. Flow for managing player max health.
            case 2:
                cursesUI[2].transform.Find("Curse").gameObject.SetActive(false);
                cursesUI[2].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 0;
                cursesUI[1].transform.Find("Curse").gameObject.SetActive(true);
                cursesUI[1].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 1;
                curCurseUI = cursesUI[activeCurses.Count];
                break;
            case 1:
                cursesUI[2].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 0;
                cursesUI[1].transform.Find("Curse").gameObject.SetActive(false);
                cursesUI[1].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 0;
                cursesUI[0].transform.Find("Curse").gameObject.SetActive(true);
                cursesUI[0].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 1;
                curCurseUI = cursesUI[activeCurses.Count];
                break;
            case 0:
                cursesUI[2].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 0;
                cursesUI[1].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 0;
                cursesUI[0].transform.Find("Curse").gameObject.SetActive(false);
                cursesUI[0].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 0;
                curCurseUI = cursesUI[activeCurses.Count];
                break;
        }

        /*
        if (activeCurses.Count < 3) {
            curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 1;
            curCurseUI = cursesUI[activeCurses.Count];
            curseMeter = 0;
            curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = curseMeter;
        }else {
            curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 1;
        }
        */
    }


    public void removeCurse()
    {
        if (activeCurses.Count == 0) {
            Debug.Log("No curses to clear!");
            return;
        }
        int i = Random.Range(0, activeCurses.Count - 1);

        
        pStats.currentHealthCap += 0.17f;
        // pStats.damage.AddBaseValue(-5.0f);
        swordRangeBlessing.baseValue -= 2.0f;
        swordRangeBlessing.updateCurse();
        if (!deathWipe) pStats.currentHealth += pStats.maxHealth * 0.17f;

        //FindObjectOfType<StatVFX>().removeCurseStat(activeCurses[i].type);

        activeCurses[i].active = true;
        activeCurses[i].isApplied = false;
        activeCurses[i].removeFlag = true;
        activeCurses.RemoveAt(i);

        switch (i) {
            case 0:
                cursesUI[0].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite;
                goto case 1;
            case 1:
                cursesUI[1].transform.Find("Curse").gameObject.GetComponent<Image>().sprite = cursesUI[2].transform.Find("Curse").gameObject.GetComponent<Image>().sprite;
                break;
        }

        newCurse = true;
    }

    public void updateCurses () {
        activeCurses.ForEach(x => x.updateCurse());
        Debug.Log("Curses updated after buying a buff!");
    }

    public void difficultyUpdateCurse (float difficulty) {
        activeCurses.ForEach(x => x.updateCurse(difficulty));
        Debug.Log("Curse difficulty updated!");
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
                    //manageCurseUI();
                    curCurseUI.transform.Find("Curse").gameObject.SetActive(true);
                    var a = x.image;
                    curCurseUI.transform.Find("Curse").gameObject.GetComponent<Image>().sprite = a;
                    cp.showCursePopup(x.type);
                    x.invokeCurse();
                    manageCurseUI();
                    //FindObjectOfType<StatVFX>().addCurseStat(x.type);
                    pStats.currentHealthCap -= 0.17f;
                    swordRangeBlessing.baseValue += 2.0f;
                    swordRangeBlessing.updateCurse();
                }
            }
        );
    }
}
