using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    [SerializeField]
    public List<Buff> Buffs = new List<Buff>();
    public List<Equipment> Equipment = new List<Equipment>();
    public CurseMeter curseMeter;

    public List<GameObject> BuffsUI = new List<GameObject>();
    public Sprite Notch, damageBuff, speedBuff, armorBuff, healthBuff;
    private bool isDaed = false;
    public Transform[] SpringTransforms;
    public float currentHealthCap = 1.0f;
    public bool noCoindens = false;
    public int moneyCurseLock = 0;

    void Start()
    {
        //set player starting coins here
        coins = 1000;
        currentHealth = maxHealth;
        for(int i = 0; i < BuffsUI.Count; i++) {
            BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {  
        //debug:
        if(Input.GetKeyDown(KeyCode.T)) Die();

        currentHealth = Mathf.Clamp(currentHealth, -0.1f, maxHealth * currentHealthCap); // Keep HP between -0.1 so death checking is ok, and the current health cap

        //death check
        if(currentHealth<=0){
            Die();
        }
        if (moneyCurseLock > coins) moneyCurseLock = coins;
        if (noCoindens) coins = moneyCurseLock;
        SoulsUI.text = "" + coins;
        if (Buffs.Count != 0)
        {
            BuffHandler (Buffs);
            for(int i = 0; i < Buffs.Count; i++){
                if(Buffs[i].timeActive < Buffs[i].duration){
                    Buffs[i].timeActive += Time.deltaTime;
                    BuffsUI[i].transform.Find("Bar").gameObject.GetComponent<Image>().fillAmount = 1 - Buffs[i].timeActive/Buffs[i].duration;
                }else{
                    BuffsUI[i].transform.Find("Bar").gameObject.GetComponent<Image>().enabled = false;
                    Buffs[i].removeFlag = true;
                }
            }
        }
    }

    public void BuffHandler(List<Buff> Buffs)
    {
        Buffs
            .ForEach(x =>
            {
                int i = Buffs.FindIndex(y => y.teaName == x.teaName);
                if (!x.isApplied)
                {
                    switch (x.stat)
                    {
                        case (Buff.statType.health):
                            {
                                this.maxHealth += x.power;
                                this.currentHealth += x.power;
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = healthBuff;
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.armor):
                            {
                                this.armor.AddBaseValue(x.power);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = armorBuff;
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.damage):
                            {
                                this.damage.AddBaseValue(x.power);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = damageBuff;
                                x.isApplied = true;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.speed):
                            {
                                this.speed.AddBaseValue(x.power);
                                BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().sprite = speedBuff;
                                x.isApplied = true;
                                curseMeter.SendMessage("updateCurses");
                                Debug.Log(Buffs.Count);
                                break;
                            }
                    }
                    BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().enabled = true;
                    BuffsUI[i].transform.Find("Bar").gameObject.GetComponent<Image>().enabled = true;
                }
                if (x.removeFlag)
                {
                    switch (x.stat)
                    {
                        case (Buff.statType.health):
                            {
                                this.maxHealth -= x.power;
                                if (this.currentHealth > this.maxHealth)
                                {
                                    this.currentHealth = this.maxHealth;
                                }
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.armor):
                            {
                                this.armor.AddBaseValue(-x.power);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.damage):
                            {
                                this.damage.AddBaseValue(-x.power);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                        case (Buff.statType.speed):
                            {
                                this.speed.AddBaseValue(-x.power);
                                x.isApplied = false;
                                Debug.Log(Buffs.Count);
                                break;
                            }
                    }
                    BuffsUI[i].transform.Find("Buff").gameObject.GetComponent<Image>().enabled = false;
                }
            });
            for(int i = 0; i < Buffs.Count; i ++){
                if(Buffs[i].removeFlag){
                    Buffs.RemoveAt(i);
                    BuffsUI.Add(BuffsUI[0]);
                    BuffsUI.RemoveAt(0);
                }
            }
    }

    public void addBuff(Buff x)
    {
        if(Buffs.Count < 3){
            x.timeActive = 0;
            x.isApplied = false;
            x.removeFlag = false;
            Buffs.Add (x);
            Debug.Log(x.teaName);
        }
    }

    public void removeBuff(Buff x)
    {
        int i = Buffs.FindIndex(y => y.teaName == x.teaName);
        Buffs[i].removeFlag = true;
        // make sure that shop buff is purchasable
        Buffs[i].isApplied = false;
    }

    //todo: add equipment handler
    public void addEquip(Equipment x)
    {
        if(Equipment.Count < 3){
            Equipment.Add(x);
            Debug.Log(x.equipName);
        }
    }

    public void removeEquip(Equipment x)
    {
        int i = Equipment.FindIndex(y => y.equipName == x.equipName);
        Equipment.RemoveAt(i);
        Debug.Log(x.equipName);
    }

    override public void TakeDamage (float damage, float knockBackStrength) {
        hitVFX.Play();
        //armor system
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, maxHealth * 0.9f);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, float.MaxValue);

        if(noCoindens) coins -= (int)((float)coins * curseMeter.kromer.GetValue());
        if(!gameObject.GetComponent<Animator>().GetBool("Death")) gameObject.GetComponent<PlayerController>().Stun();
    }

    override public void Die () {
        Debug.Log("Player died!");
        StartCoroutine(PlayerDeath(this.gameObject));
    }

    public IEnumerator PlayerDeath(GameObject player) {
        curseMeter.deathWipe = true;
        foreach(Buff x in Buffs) removeBuff(x);
        // turning the lockon camera off in the case that it's on while dying
        GetComponent<LockTarget>().LockOnCamera.SetActive(false);
        // disable player move script
        // play death animation
        deathUI.SetActive(true);
        player.GetComponent<Animator>().SetBool("Death", true);
        player.GetComponent<CharacterController>().enabled = false;
        Transform[] springTransforms = FindObjectOfType<PlayerStats>().SpringTransforms;
        Vector3 respawnPosition = Vector3.zero;
        float minMagnitude = float.MaxValue;
        for(int i = 0; i < springTransforms.Length; i ++){
            float mag = (player.transform.position - springTransforms[i].transform.position).magnitude;
            if(mag < minMagnitude){
                minMagnitude = mag;
                respawnPosition = springTransforms[i].position;
            }
        }
        yield return new WaitForSeconds(3f);
        player.GetComponent<PlayerController>().AnimationStart();
        player.GetComponent<Animator>().SetBool("Death", false);
        player.transform.position = respawnPosition;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<PlayerStats>().currentHealth = player.GetComponent<PlayerStats>().maxHealth;
        curseMeter.deathWipe = false;
        yield return null;
    }
}
