using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NewShopManager : MonoBehaviour
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

    public GameObject displaySprite;
    // public SpriteRenderer currentSprite;

  

    public int selectedOption = 0;

    // Start is called before the first frame update
    void Start()
    {
        //refresh buffs
        foreach(Buff i in shopBuffList){
            i.isApplied = false;
        }
        menuCoinTXT.text = "Coins:" + playStats.coins.ToString();
        UICoinTXT.text = "Coins:" + playStats.coins.ToString();
        UpdateDisplay (selectedOption);
    }

    public void NextOption()
    {
        selectedOption++;
        if (selectedOption >= shopBuffList.Count)
        {
            selectedOption = 0;
        }

        UpdateDisplay (selectedOption);
    }

    public void BackOption()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = shopBuffList.Count - 1;
        }

        UpdateDisplay (selectedOption);
    }

    public void Buy(){
        if(playStats.coins>= currentBuff.Cost && !currentBuff.isApplied &&playStats.Buffs.Count<3){
            //update coins
            playStats.coins -= currentBuff.Cost;
            menuCoinTXT.text = "Coins:" + playStats.coins.ToString();
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();
            //add buffs to player
            playStats.addBuff(currentBuff);

        }
    }
    public void Upgrade(){
        if(playStats.coins>= currentBuff.InvestCost && currentBuff.duration<=400){
            //update coins
            playStats.coins -= currentBuff.InvestCost;
            menuCoinTXT.text = "Coins:" + playStats.coins.ToString();
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();
            //add duration to buffs
            currentBuff.duration += 100;
        }
    }
    public void Exit(){
        GameObject.FindGameObjectWithTag("TeaShop").GetComponent<TeaShop>().CloseMenu();
    }
    public void UpdateDisplay(int selectedOption)
    {
        currentBuff = shopBuffList[selectedOption];
        displaySprite.GetComponent<Image>().sprite = currentBuff.buffSprite;
        buffName.text = currentBuff.name;
        description.text = currentBuff.description;
        costTXT.text = "$: "+ currentBuff.Cost.ToString();
        investCostTXT.text = "$: "+ currentBuff.InvestCost.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
