using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// using static Equip;
public class EquipShopManager : MonoBehaviour
{
    public Sprite

            equipSprite;


    public List<Equipment> shopEquipList = new List<Equipment>();

    public PlayerStats playStats;

    public Text UICoinTXT;

    public Text menuCoinTXT;

    public Text costTXT;

    public Text investCostTXT;

    public Text equipName;

    public Text description;

    public Equipment currentEquip;

    public Equipment prevEquip;

    public Equipment nextEquip;

    public GameObject displayPrev;

    public GameObject display;

    public GameObject displayNext;

    // public SpriteRenderer currentSprite;
    public int selectedOption = 0;

    public AudioClip purchase;

    public AudioClip cantafford;

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        testWeapon testEquip = new testWeapon(equipSprite);
        

        shopEquipList.Add(testEquip);

        UpdateDisplay(0);
    }

    public void NextOption()
    {
        selectedOption++;
        if (selectedOption >= shopEquipList.Count)
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
            selectedOption = shopEquipList.Count - 1;
        }

        UpdateDisplay (selectedOption);
    }

    public void Buy()
    {
        if (
            playStats.coins >= currentEquip.Cost &&
            !currentEquip.isEquipped &&
            playStats.Equipment.Count < 3 &&
            GameObject
                .FindGameObjectWithTag("EquipmentShop")
                .GetComponent<equipmentShop>()
                .isOpen
        )
        {
            //update coins
            playStats.coins -= currentEquip.Cost;
            // menuCoinTXT.text = "Coins:" + playStats.coins.ToString();
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();

            //add Equipment to player
            //Todo: add to player
            playStats.addEquip(currentEquip);
            Purchase();
        }
        else
        {
            Cantafford();
        }
    }

    public void Upgrade()
    {
        if (
            playStats.coins >= currentEquip.InvestCost &&
            currentEquip.duration <= 400
        )
        {
            //update coins
            playStats.coins -= currentEquip.InvestCost;
            menuCoinTXT.text = "Coins:" + playStats.coins.ToString();
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();

            //add duration to Equips
            currentEquip.duration += 100;
            Purchase();
        }
        else
        {
            Cantafford();
        }
    }

    public void Exit()
    {
        
        GameObject
            .FindGameObjectWithTag("EquipmentShop")
            .GetComponent<equipmentShop>()
            .CloseMenu();
    }

    public void UpdateDisplay(int selectedOption)
    {
        currentEquip = shopEquipList[selectedOption];

        if (selectedOption == 0)
        {
            prevEquip = shopEquipList[shopEquipList.Count - 1];
        }
        else
        {
            prevEquip = shopEquipList[selectedOption - 1];
        }

        if (selectedOption == shopEquipList.Count - 1)
        {
            nextEquip = shopEquipList[0];
        }
        else
        {
            nextEquip = shopEquipList[selectedOption + 1];
        }

        
        display.GetComponent<Image>().sprite = currentEquip.equipSprite;
        displayPrev.GetComponent<Image>().sprite = prevEquip.equipSprite;
        displayNext.GetComponent<Image>().sprite = nextEquip.equipSprite;
        equipName.text = currentEquip.equipName;
        description.text = currentEquip.description;
        costTXT.text = "$: " + currentEquip.Cost.ToString();
        investCostTXT.text = "$: " + currentEquip.InvestCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (
            GameObject
                .FindGameObjectWithTag("EquipmentShop")
                .GetComponent<equipmentShop>()
                .isOpen
        )
        {
            if (
                Input.GetKeyDown(KeyCode.JoystickButton4) ||
                Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.A)
            )
            {
                BackOption();
            }
            if (
                Input.GetKeyDown(KeyCode.JoystickButton5) ||
                Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.D)
            )
            {
                NextOption();
            }
            if (Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                Exit();
            }
        }
    }

    //Ethan's code:
    public void Purchase()
    {
        audioSource.pitch = Random.Range(1f, 2f);
        audioSource.PlayOneShot(purchase);
    }

    public void Cantafford()
    {
        audioSource.PlayOneShot(cantafford);
    }

    private AudioClip GetRandomClip(AudioClip[] cliparray)
    {
        return cliparray[Random.Range(0, cliparray.Length)];
    }
}
