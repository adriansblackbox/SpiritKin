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
    private PlayerStats pStats;

    public bool newCurse;

    public float curseMeter;
    public float fillRate; // Value is inverse to rate
    public List<Curse> curseArray = new List<Curse>();
    public List<Curse> activeCurses = new List<Curse>();
    public GameObject[] cursesUI;
    public GameObject Sword1, Sword2, Sword3;
    private GameObject curCurseUI;
    public Sprite Notch, weakImage, slowImage, frailImage, blindImage;
    public GameObject ActiveSword;
    public GameObject[] shrines;
    public GameObject blindVignette;
    public GameObject healthBar;

    // Needed Curse Obtained Popup
    public GameObject cursePopup;
    private CursePopup cp;

    // Bonus sword damage & current health capping?
    public float[] bonusDamages;
    public bool[] bonusDamagesActive;

    public bool debugbool = false;

    // Start is called before the first frame update
    private void Start() {
        newCurse = false;
        curseMeter = 0f;
        pStats = gameObject.GetComponent<PlayerStats>();

        damageCurse weak = new damageCurse(weakImage, gameObject.GetComponent<PlayerStats>(), this);
        slowCurse slow = new slowCurse(slowImage, gameObject.GetComponent<PlayerStats>(), this);
        armorCurse frail = new armorCurse(frailImage, gameObject.GetComponent<PlayerStats>(), this);
        blindCurse blind = new blindCurse(blindImage, this, shrines, blindVignette, healthBar);

        curseArray.Add(weak);
        curseArray.Add(slow);
        curseArray.Add(frail);
        curseArray.Add(blind);

        curCurseUI = cursesUI[0];
        curCurseUI.transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = curseMeter;
        // ActiveSword = Sword0;
        Sword1.SetActive(false); Sword2.SetActive(false); Sword3.SetActive(false);

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

        if (debugbool) { // Debug to remove all curses
            removeCurse();
            //debugbool = !debugbool;
        }

        if (newCurse) { // If this is flipped, we need to update something about curses
            CurseHandler();
            newCurse = false;
            HandleSword();
        }
    }

    private void HandleSword () {
        switch (activeCurses.Count) {
            case 0:
                // Sword0.SetActive(true);
                Sword1.SetActive(false);
                Sword2.SetActive(false);
                Sword3.SetActive(false);
                //handleSwordDamage();
                // ActiveSword = Sword0;
                break;
            case 1:
                Sword1.SetActive(true);
                // Sword0.SetActive(false);
                Sword2.SetActive(false);
                Sword3.SetActive(false);
                //handleSwordDamage();
                ActiveSword = Sword1;
                //pStats.damage.AddBaseValue(bonusDamages[1]);
                //bonusDamagesActive[1] = true;
                break;
            case 2:
                Sword2.SetActive(true);
                Sword1.SetActive(false);
                // Sword0.SetActive(false);
                Sword3.SetActive(false);
                //handleSwordDamage();
                ActiveSword = Sword2;
                //pStats.damage.AddBaseValue(bonusDamages[2]);
                //bonusDamagesActive[2] = true;
                break;
            case 3:
                Sword3.SetActive(true);
                Sword1.SetActive(false);
                Sword2.SetActive(false);
                // Sword0.SetActive(false);
                //handleSwordDamage();
                ActiveSword = Sword3;
                //pStats.damage.AddBaseValue(bonusDamages[3]);
                //bonusDamagesActive[3] = true;
                break;
        }
    }

    private void handleSwordDamage () {
        if (ActiveSword == Sword1) {
            pStats.damage.AddBaseValue(-bonusDamages[1]);
            bonusDamagesActive[1] = false;
            // currentHealthCap = 75%
        }
        else if (ActiveSword == Sword2) {
            pStats.damage.AddBaseValue(-bonusDamages[2]);
            bonusDamagesActive[2] = false;
            // currentHealthCap = 50%
        }
        else { 
            pStats.damage.AddBaseValue(-bonusDamages[3]);
            bonusDamagesActive[3] = false;
            // currentHealthCap = 25%
        }
    }

    public void addCurse() {
        List<Curse> unactiveCurses = curseArray.Except(activeCurses).ToList();
        if (unactiveCurses.Count > 0) {
            pStats.currentHealthCap -= 0.17f;
            pStats.damage.AddBaseValue(5.0f);
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
                cursesUI[1].transform.Find("Curse").gameObject.SetActive(false);
                cursesUI[1].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 0;
                cursesUI[0].transform.Find("Curse").gameObject.SetActive(true);
                cursesUI[0].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 1;
                curCurseUI = cursesUI[activeCurses.Count];
                break;
            case 0:
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
        pStats.damage.AddBaseValue(-5.0f);
        pStats.currentHealth += pStats.maxHealth * 0.17f;

        FindObjectOfType<StatVFX>().removeCurseStat(activeCurses[i].type);

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
        activeCurses.ForEach(x => x.updateCurse(x.penaltyValue * difficulty));
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
                    manageCurseUI();
                    curCurseUI.transform.Find("Curse").gameObject.SetActive(true);
                    var a = x.image;
                    curCurseUI.transform.Find("Curse").gameObject.GetComponent<Image>().sprite = a;
                    cp.showCursePopup(x.type);
                    x.invokeCurse();
                    manageCurseUI();
                    FindObjectOfType<StatVFX>().addCurseStat(x.type);
                }
            }
        );
    }
}
