using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManagerScript : MonoBehaviour
{
    public int[,] shopItems = new int[5, 5];

    public List<Buff> shopBuffList = new List<Buff>();

    public PlayerStats playStats;

    public Text CoinsTXT;

    public Text CoinsUITXT;

    // Start is called before the first frame update
    void Start()
    {
        CoinsTXT.text = "Coins:" + playStats.coins.ToString();

        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;
        shopItems[1, 4] = 4;

        //price
        shopItems[2, 1] = 10;
        shopItems[2, 2] = 20;
        shopItems[2, 3] = 30;
        shopItems[2, 4] = 40;

        //quantity
        shopItems[3, 1] = 0;
        shopItems[3, 2] = 0;
        shopItems[3, 3] = 0;
        shopItems[3, 4] = 0;
    }

    public void Buy()
    {
        GameObject ButtonRef =
            GameObject
                .FindGameObjectWithTag("Event")
                .GetComponent<EventSystem>()
                .currentSelectedGameObject;
        if (
            playStats.coins >=
            shopItems[2, ButtonRef.GetComponent<button_info>().ItemID] &&
            shopItems[3, ButtonRef.GetComponent<button_info>().ItemID] <= 0
        )
        {
            playStats.coins -=
                shopItems[2, ButtonRef.GetComponent<button_info>().ItemID];
            shopItems[3, ButtonRef.GetComponent<button_info>().ItemID]++;
            CoinsTXT.text = "Coins:" + playStats.coins.ToString();
            
            ButtonRef.GetComponent<button_info>().BuyTxt.text = "Sold";
                shopItems[3, ButtonRef.GetComponent<button_info>().ItemID]
                    .ToString();

            //add buff to player
            GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<PlayerStats>()
                .addBuff(shopBuffList[ButtonRef
                    .GetComponent<button_info>()
                    .ItemID -
                1]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
