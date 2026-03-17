using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    public GameObject shop;
    public Transform shopItemsParent;
    public GameObject shopItemPrefab;

    [Header("Shop Item Details")]
    public TextMeshProUGUI shopItemName;
    public TextMeshProUGUI shopItemDescription;
    public TextMeshProUGUI shopItemPrice;
    public Image shopItemIcon;

    [Header("Shop Confirm")]
    public GameObject shopConfirm;
    public TextMeshProUGUI shopConfirmText;
    public TextMeshProUGUI shopConfirmPrice;
    public Image shopConfirmIcon;
    public Button shopConfirmButton;
    public Button shopDeclineButton;


    [Header("Dialogue")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI dialogueText;
    private Queue<string> lines = new Queue<string>();
    private bool dialogueEnded;
    private bool typing;
    private string line;
    private Coroutine dialogueCoroutine;
    private NPCDialogue currentNPC;

    [Header("Upgrade")]
    public CanvasGroup upgradeScreen;
    public TextMeshProUGUI obtainText;
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI upgradeDescText;
    public Image upgradeImage;

    [Header("Map")]
    public GameObject mapScreen;
    public Transform mapParent;
    public GameObject mapRoom;

    private GameObject player;
    public Animator healFactor;

    [Header("Obtain Object")]
    public GameObject obtainObject;
    public TextMeshProUGUI obtainObjectText;
    public Image obtainObjectImage;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        moneyText.text = MainManager.Instance.money.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !MainManager.Instance.shopping)
            ToggleMap();

        if (Input.GetKeyDown(KeyCode.Escape) && MainManager.Instance.shopping)
        {
            CloseShop(false);
        }
    }

    public void Heal()
    {
        healFactor.Play("Heal");
    }

    public void ToggleMap()
    {
        if(!mapScreen.activeSelf)
        {
            foreach(Transform t in mapParent)
                Destroy(t.gameObject);

            RoomManager[] rooms = FindObjectsByType<RoomManager>(FindObjectsSortMode.None);

            foreach(RoomManager room in rooms)
            {
                if(room.visited)
                {
                    GameObject coll = room.camCollider;
                    GameObject newRoom = Instantiate(mapRoom, transform.position, transform.rotation);
                    newRoom.transform.SetParent(mapParent, false);
                    newRoom.transform.localScale = coll.transform.localScale * 3;
                    newRoom.transform.localPosition = coll.transform.position * 3;
                    newRoom.name = room.gameObject.name;
                }
            }
        }

        mapScreen.SetActive(!mapScreen.activeSelf);
    }

    public void OpenShop(ShopItemData[] items, List<int> boughtItems, ShopMerchant merchant)
    {
        MainManager.Instance.shopping = true;

        foreach (Transform c in shopItemsParent)
            Destroy(c.gameObject);

        for (int i = 0; i < items.Length; i++)
        {
            if (!boughtItems.Contains(i))
            {
                GameObject newItem = Instantiate(shopItemPrefab, shopItemsParent);
                newItem.transform.SetParent(shopItemsParent, false);
                newItem.GetComponent<ShopItem>().SetItem(items[i], merchant, i);
            }
        }

        StopPlayer();

        StartCoroutine(SelectShopItem());
        DisplayShopItem(shopItemsParent.GetChild(0).gameObject.GetComponent<ShopItem>().data);
        shop.SetActive(true);
    }

    private void StopPlayer()
    {
        player.GetComponent<PlayerAttack>().canAttack = false;
        player.GetComponent<PlayerMovement>().canMove = false;
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
    }

    private IEnumerator SelectShopItem()
    {
        yield return new WaitForEndOfFrame();
        shopItemsParent.GetChild(0).gameObject.GetComponent<Button>().Select();
    }

    public void DisplayShopItem(ShopItemData data)
    {
        shopItemName.text = data.displayName;
        shopItemDescription.text = data.description;
        shopItemPrice.text = data.price.ToString();
        shopItemIcon.sprite = data.icon;
    }

    public void ConfirmPurchasePopUp(ShopItem item)
    {
        shop.SetActive(false);

        shopConfirmText.text = "Buy " + item.data.displayName + "?";
        shopConfirmPrice.text = item.data.price.ToString();
        shopConfirmIcon.sprite = item.data.icon;
        shopConfirmButton.Select();

        shopConfirmButton.onClick.RemoveAllListeners();
        shopConfirmButton.onClick.AddListener(delegate { BuyShopItem(item); });

        shopDeclineButton.onClick.RemoveAllListeners();
        shopDeclineButton.onClick.AddListener(delegate { CancelShop(); });

        shopConfirm.SetActive(true);
    }

    public void CancelShop()
    {
        shopConfirm.SetActive(false);
        StartCoroutine(SelectShopItem());
        shop.SetActive(true);
    }

    public void CloseShop(bool finalItem)
    {
        shop.SetActive(false);
        shopConfirm.SetActive(false);
        player.GetComponent<PlayerAttack>().canAttack = true;
        player.GetComponent<PlayerMovement>().canMove = true;

        MainManager.Instance.shopping = false;

        if(!finalItem)
            currentNPC.OnShopEnd();
        else
            currentNPC.OnEmptyShop();
    }

    public void BuyShopItem(ShopItem item)
    {
        ShopItemData data = item.data;
        GetMoney(-data.price);

        item.merchant.boughtItems.Add(item.itemIndex);

        ObtainObject(data.itemName, data.icon);

        CloseShop(item.merchant.boughtItems.Count == item.merchant.items.Length);
    }

    public void GetMoney(int amount)
    {
        MainManager.Instance.money += amount;
        moneyText.text = MainManager.Instance.money.ToString();
    }

    public void DisplayNextLine(DialogueText dialogueText, NPCDialogue npc)
    {
        currentNPC = npc;
        if(lines.Count == 0)
        {
            if (!dialogueEnded)
                StartDialogue(dialogueText);
            else if (dialogueEnded && !typing)
            {
                EndDialogue();
                return;
            }
        }

        if(!typing)
        {
            line = lines.Dequeue();
            dialogueCoroutine = StartCoroutine(TypeDialogueText(line));
        }else
        {
            FinishLineEarly();
        }

        if (lines.Count == 0)
        {
            dialogueEnded = true;
        }
    }

    private void StartDialogue(DialogueText dialogueText)
    {
        StopPlayer();

        dialogueBox.SetActive(true);
        dialogueName.text = dialogueText.speakerName;

        for (int i = 0; i < dialogueText.lines.Length; i++)
        {
            lines.Enqueue(dialogueText.lines[i]);
        }
    }

    private void EndDialogue()
    {
        player.GetComponent<PlayerAttack>().canAttack = true;
        player.GetComponent<PlayerMovement>().canMove = true;
        lines.Clear();
        dialogueEnded = false;
        dialogueBox.SetActive(false);

        currentNPC.OnDialogueEnd();
    }

    private IEnumerator TypeDialogueText(string line)
    {
        typing = true;

        dialogueText.text = "";
        string originalText = line;
        string displayText = "";
        int alpha = 0;

        foreach (char c in line.ToCharArray())
        {
            alpha++;
            dialogueText.text = originalText;

            displayText = dialogueText.text.Insert(alpha, "<color=#00000000>");
            dialogueText.text = displayText;

            yield return new WaitForSeconds(0.03f);
        }

        typing = false;
    }

    private void FinishLineEarly()
    {
        StopCoroutine(dialogueCoroutine);
        dialogueText.text = line;
        typing = false;
    }

    public void ObtainObject(string objName, Sprite image)
    {
        string obtain, upName, upDesc;

        switch(objName)
        {
            case "dash": 
                player.GetComponent<PlayerMovement>().hasDash = true;
                MainManager.Instance.hasDash = true;

                obtain = "Obtained";
                upName = "Phase Shifter";
                upDesc = "Increases your movement options.\nPress C to dash forward.";

                DisplayUpgrade(obtain, upName, upDesc);
                break;
            case "doubleJump":
                player.GetComponent<PlayerMovement>().hasDoubleJump = true;
                MainManager.Instance.hasDoubleJump = true;

                obtain = "Obtained";
                upName = "Boot Module";
                upDesc = "Increases your maximum height.\nPress SPACE in the air to double jump.";

                DisplayUpgrade(obtain, upName, upDesc);
                break;
            case "battery":
                player.GetComponent<PlayerAttack>().IncreaseMaxHealCount();

                DisplayObject("Battery Slot", image);
                break;
            case "mineral":
                MainManager.Instance.minerals++;
                DisplayObject("Mineral", image);
                break;
            case "armorPiece":
                MainManager.Instance.hpPieces++;
                if (MainManager.Instance.hpPieces % 3 == 0)
                    player.GetComponent<PlayerHealth>().IncreaseHp();

                DisplayObject("Amour Piece", image);
                break;
        }
    }

    public void DisplayObject(string objName, Sprite image)
    {
        obtainObjectText.text = objName;
        //obtainObjectImage.sprite = image;

        obtainObject.SetActive(true);

        StartCoroutine(CloseItem());
    }

    private IEnumerator CloseItem()
    {
        yield return new WaitForSeconds(2);
        obtainObject.SetActive(false);
    }

    public void DisplayUpgrade(string obtain, string upName, string upDesc)
    {
        StopPlayer();

        obtainText.text = obtain;
        upgradeNameText.text = upName;
        upgradeDescText.text = upDesc;

        StartCoroutine(ShowUpgrade());
    }

    private IEnumerator ShowUpgrade()
    {
        yield return new WaitForSeconds(1.5f);

        upgradeScreen.gameObject.SetActive(true);
        upgradeScreen.alpha = 0;

        for(float i = 0; i < 1; i += 0.05f)
        {
            upgradeScreen.alpha = i;
            yield return new WaitForSeconds(0.05f);
        }

        upgradeScreen.alpha = 1;

        yield return new WaitForSeconds(3);

        upgradeScreen.alpha = 0;
        upgradeScreen.gameObject.SetActive(false);
        player.GetComponent<PlayerAttack>().canAttack = true;
        player.GetComponent<PlayerMovement>().canMove = true;
    }

    public void UpgradeWeaponPopUp(HealStation station)
    {
        StopPlayer();

        shopConfirmText.text = "Give Mineral?";
        shopConfirmPrice.text = "1";
        //shopConfirmIcon.sprite = item.data.icon;
        shopConfirmButton.Select();

        shopConfirmButton.onClick.RemoveAllListeners();
        shopConfirmButton.onClick.AddListener(delegate { UpgradeWeapon(station); });

        shopDeclineButton.onClick.RemoveAllListeners();
        shopDeclineButton.onClick.AddListener(delegate { CancelUpgrade(); });

        shopConfirm.SetActive(true);
    }

    public void CancelUpgrade()
    {
        shopConfirm.SetActive(false);
        player.GetComponent<PlayerAttack>().canAttack = true;
        player.GetComponent<PlayerMovement>().canMove = true;
    }

    public void UpgradeWeapon(HealStation station)
    {
        if(MainManager.Instance.minerals > 0)
        {
            station.used = true;
            MainManager.Instance.minerals--;
            MainManager.Instance.weaponLevel++;
            shopConfirm.SetActive(false);
            player.GetComponent<PlayerAttack>().canAttack = true;
            player.GetComponent<PlayerMovement>().canMove = true;

            DisplayObject("Weapon Upgraded", null);
        }
    }
}
