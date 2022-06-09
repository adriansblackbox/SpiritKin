using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// using static Buff;
public class TeaShopManager : MonoBehaviour
{
    // [SerializeField] PlayerController playerController;

    public Sprite

            armorBuffSprite,
            damageBuffSprite,
            healthBuffSprite,
            speedBuffSprite;

    public List<Buff> shopBuffList = new List<Buff>();

    public PlayerStats playStats;

    public Text UICoinTXT;

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

    public AudioClip purchase, cantafford, hover;

    [SerializeField]
    private AudioClip[] shopexits;

    [SerializeField]
    private AudioClip[] shopchanges;

    public AudioSource audioSource;

    private PlayerData pd;

    [SerializeField] private CurseMeter cm;


    // Start is called before the first frame update
    void Start()
    {
        pd = FindObjectOfType<PlayerData>();
    }

    void Awake()
    {
        ArmorBuff armorBuff = new ArmorBuff(armorBuffSprite);
        DamageBuff damageBuff = new DamageBuff(damageBuffSprite);
        SpeedBuff speedBuff = new SpeedBuff(speedBuffSprite);
        HealthBuff healthBuff = new HealthBuff(healthBuffSprite);

        shopBuffList.Add (armorBuff);
        shopBuffList.Add (damageBuff);
        shopBuffList.Add (speedBuff);
        shopBuffList.Add (healthBuff);
        UpdateDisplay(0);
    }

    public void NextOption()
    {
        selectedOption++;
        if (selectedOption >= shopBuffList.Count)
        {
            selectedOption = 0;
        }
        Change();
        UpdateDisplay (selectedOption);
    }

    public void BackOption()
    {
        selectedOption--;
        if (selectedOption < 0)
        {
            selectedOption = shopBuffList.Count - 1;
        }
        Change();
        UpdateDisplay (selectedOption);
    }

    public void Buy()
    {
        if (
            playStats.coins >= currentBuff.Cost &&
            !currentBuff.isApplied &&
            playStats.Buffs.Count < 3 &&
            GameObject
                .FindGameObjectWithTag("TeaShop")
                .GetComponent<TeaShop>()
                .isOpen
        )
        {
            Purchase();
            //update coins
            playStats.coins -= currentBuff.Cost;
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();
            pd.addBuffsPurchased(1);

            //add buffs to player
            playStats.addBuff (currentBuff);
            cm.updateCurses();
        }
        // Allow the player to re-purchase a buff if they have expended more than 25% of its duration
        else if (
            playStats.coins >= currentBuff.Cost &&
            currentBuff.isApplied &&
            currentBuff.timeActive / currentBuff.duration > 0.25f
        ) 
        {
            
            Purchase();
            //update coins
            playStats.coins -= currentBuff.Cost;
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();
            pd.addBuffsPurchased(1);

            //add buffs to player
            playStats.refreshBuff(currentBuff);
            cm.updateCurses();
        }
        else
        {
            //Debug.Log(currentBuff.timeActive / currentBuff.duration);
            Cantafford();
        }
        // playerController.enabled = false;
    }

    public void Upgrade()
    {
        if (
            playStats.coins >= currentBuff.investCost && currentBuff.level < 3 //3 is the max level
        )
        {
            //update coins
            playStats.coins -= currentBuff.investCost;
            UICoinTXT.text = "Coins:" + playStats.coins.ToString();

            //make buff stronger
            currentBuff.level += 1; //level gets increased by 1s
            currentBuff.duration += 60; //duration gets increased by a minute
            currentBuff.power += currentBuff.basePower * currentBuff.level;
            Purchase();
            if (currentBuff.level < 3)
            {
                currentBuff.investCost += 150;
                investCostTXT.text = "$: " + currentBuff.investCost.ToString();
                currentBuff.updateDescription();
                description.text = currentBuff.description;
                cm.updateCurses();
                playStats.updateBuffStrength(currentBuff);
            }
            else
            {
                //disable upgrade button rather than saying max or maybe do both
                investCostTXT.text = "Max";
                description.text = "Maximum potency reached";
            }
        }
        else
        {
            Cantafford();
        }
        // playerController.enabled = false;
    }

    public void Exit()
    {
        Shopexit();
        GameObject
            .FindGameObjectWithTag("TeaShop")
            .GetComponent<TeaShop>()
            .CloseMenu();
    }

    public void UpdateDisplay(int selectedOption)
    {
        currentBuff = shopBuffList[selectedOption];

        if (selectedOption == 0)
        {
            prevBuff = shopBuffList[shopBuffList.Count - 1];
        }
        else
        {
            prevBuff = shopBuffList[selectedOption - 1];
        }

        if (selectedOption == shopBuffList.Count - 1)
        {
            nextBuff = shopBuffList[0];
        }
        else
        {
            nextBuff = shopBuffList[selectedOption + 1];
        }

        // nextBuff = shopBuffList[selectedOption+1];
        // nextBuff = shopBuffList[selectedOption-1];
        display.GetComponent<Image>().sprite = currentBuff.buffSprite;
        displayPrev.GetComponent<Image>().sprite = prevBuff.buffSprite;
        displayNext.GetComponent<Image>().sprite = nextBuff.buffSprite;
        buffName.text = currentBuff.teaName;
        costTXT.text = "$: " + currentBuff.Cost.ToString();

        if (currentBuff.level < 3)
        {
            description.text = currentBuff.description;
            investCostTXT.text = "$: " + currentBuff.investCost.ToString();
        }
        else
        {
            investCostTXT.text = "Max";
            description.text = "Maximum potency reached";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (
            GameObject
                .FindGameObjectWithTag("TeaShop")
                .GetComponent<TeaShop>()
                .isOpen
        )
        {
            if (
                Input.GetKeyDown(KeyCode.JoystickButton4) ||
                Input.GetKeyDown(KeyCode.LeftArrow)||
                Input.GetKeyDown(KeyCode.A)
            )
            {
                BackOption();
            }
            if (
                Input.GetKeyDown(KeyCode.JoystickButton5) ||
                Input.GetKeyDown(KeyCode.RightArrow)||
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
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(purchase);
    }

    public void Hover()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(hover);
    }

    public void Cantafford()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(cantafford);
    }

    
    public void Change()
    {
        AudioClip clip = GetRandomClip(shopchanges);
        audioSource.PlayOneShot(clip);
    }
    public void Shopexit()
    {
        AudioClip clip = GetRandomClip(shopexits);
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip);
    }
    private AudioClip GetRandomClip(AudioClip[] cliparray)
    {
        return cliparray[Random.Range(0, cliparray.Length)];
    }
}
    