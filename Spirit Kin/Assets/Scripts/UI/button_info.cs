using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class button_info : MonoBehaviour
{
    public int ItemID;
    public Text PriceTxt;
    public Text BuyTxt;
    public Text QuantityTxt;
    public GameObject ShopManager;

    public bool isSold = false;

    // Update is called once per frame
    void Update()
    {
        if(isSold){
            BuyTxt.text = "Upgrade";
        }
        PriceTxt.text = "Price: $" +ShopManager.GetComponent<ShopManagerScript>().shopItems[2,ItemID].ToString();
        QuantityTxt.text = ShopManager.GetComponent<ShopManagerScript>().shopItems[3,ItemID].ToString();
    }
}
