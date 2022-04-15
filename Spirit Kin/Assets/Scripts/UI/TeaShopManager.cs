using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TeaShopManager : MonoBehaviour
{
    public List<Buff> shopBuffList = new List<Buff>();
    public PlayerStats playStats;
    public Text UICoinTXT;
    public Text menuCoinTXT;

    public Text costTXT;
    public Text investCostTXT;

    public Text buffName;

    public Text description;

    public Buff currentBuff;
    public Buff prevBuff;
    public Buff nextBuff;

    public GameObject displayPrev;
    public GameObject display; 
    public GameObject displayNext;
    // public SpriteRenderer currentSprite;



    public int selectedOption = 0;

    [SerializeField]
    private AudioClip[] Purchaseclips;

    public AudioClip ButtonHoversfx;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        //refresh buffs
        foreach (Buff i in shopBuffList)
        {
            i.isApplied = false;
        }
        menuCoinTXT.text = "Coins:" + playStats.coins.ToString();
        UICoinTXT.text = "Coins:" + playStats.coins.ToString();
        UpdateDisplay(0);
    }
    void Awake(){
        UpdateDisplay(0);
    }
    
    public void Initial(){
        UpdateDisplay(0);
    }
    public void NextOption()
    {
        selectedOption++;
        if (selectedOption >= shopBuffList.Count)
        {
            selectedOption = 0;
        }

        UpdateDisplay(selectedOption);
    }

    public void BackOption()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = shopBuffList.Count - 1;
        }

        UpdateDisplay(selectedOption);
    }

    public void Buy()
    {
        if (playStats.coins >= currentBuff.Cost && !currentBuff.isApplied && playStats.Buffs.Count < 3)
        {
            //update coins
            playStats.coins -= currentBuff.Cost;
            menuCoinTXT.text = "Coins:" + playStats.coins.ToString();
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();
            //add buffs to player
            playStats.addBuff(currentBuff);

        }
    }
    public void Upgrade()
    {
        if (playStats.coins >= currentBuff.InvestCost && currentBuff.duration <= 400)
        {
            //update coins
            playStats.coins -= currentBuff.InvestCost;
            menuCoinTXT.text = "Coins:" + playStats.coins.ToString();
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();
            //add duration to buffs
            currentBuff.duration += 100;
        }
    }
    public void Exit()
    {
        GameObject.FindGameObjectWithTag("TeaShop").GetComponent<TeaShop>().CloseMenu();
    }
    public void UpdateDisplay(int selectedOption)
    {
        currentBuff = shopBuffList[selectedOption];

        if(selectedOption==0){
            prevBuff = shopBuffList[shopBuffList.Count-1];
        }else{
            prevBuff = shopBuffList[selectedOption-1];
        }

        if(selectedOption == shopBuffList.Count-1){
            nextBuff = shopBuffList[0];
        }else{
            nextBuff = shopBuffList[selectedOption+1];
        }
        // nextBuff = shopBuffList[selectedOption+1];
        // nextBuff = shopBuffList[selectedOption-1];
        display.GetComponent<Image>().sprite = currentBuff.buffSprite;
        displayPrev.GetComponent<Image>().sprite = prevBuff.buffSprite;
        displayNext.GetComponent<Image>().sprite = nextBuff.buffSprite;
        buffName.text = currentBuff.name;
        description.text = currentBuff.description;
        costTXT.text = "$: " + currentBuff.Cost.ToString();
        investCostTXT.text = "$: " + currentBuff.InvestCost.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("TeaShop").GetComponent<TeaShop>().isOpen){
            if(Input.GetKeyDown(KeyCode.JoystickButton4)||Input.GetKeyDown(KeyCode.LeftArrow)){
            BackOption();
        }
        if(Input.GetKeyDown(KeyCode.JoystickButton5)||Input.GetKeyDown(KeyCode.RightArrow)){
            NextOption();
        }
        if(Input.GetKeyDown(KeyCode.JoystickButton1)){
            Exit();
        }


        }
        
    }

    //Ethan's code:
    private void Hover()
    {
        audioSource.PlayOneShot(ButtonHoversfx);
    }


    private AudioClip GetRandomClip(AudioClip[] cliparray)
    {
        return cliparray[Random.Range(0, cliparray.Length)];
    }
}
