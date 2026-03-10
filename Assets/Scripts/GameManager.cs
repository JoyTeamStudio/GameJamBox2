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


    private GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        moneyText.text = MainManager.Instance.money.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && MainManager.Instance.shopping)
        {
            CloseShop(false);
        }
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

        switch(data.itemName)
        {
            case "battery":
                player.GetComponent<PlayerAttack>().IncreaseMaxHealCount();
                break;
            case "armorPiece":
                MainManager.Instance.hpPieces++;
                if (MainManager.Instance.hpPieces % 3 == 0)
                    player.GetComponent<PlayerHealth>().IncreaseHp();

                break;
        }

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
}
