using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool paused;
    public bool startedGame;
    public bool endedGame;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI bossText;

    public GameObject shop;
    public Transform shopItemsParent;
    public GameObject shopItemPrefab;

    public Transform healthPieces;

    public Image fade;
    public Sprite mapIcon;

    public GameObject finalItem;

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
    public Image shopConfirmPriceIcon;
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
    private DialogueText currentDialogueText;

    [Header("Upgrade")]
    public CanvasGroup upgradeScreen;
    public TextMeshProUGUI obtainText;
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI upgradeDescText;
    public Image upgradeImage;
    public Sprite upgradeIcon;

    [Header("Map")]
    public GameObject mapScreen;
    public Transform mapParent;
    public GameObject mapRoom;
    public Transform playerImage;
    public Transform objectiveImage;

    private GameObject player;
    public Animator healFactor;

    [Header("Obtain Object")]
    public GameObject obtainObject;
    public TextMeshProUGUI obtainObjectText;
    public Image obtainObjectImage;

    public GameObject endBoxDialogue;

    [Header("Start Game")]
    public Door startDoor;
    public GameObject finalInteract;
    public TextMeshProUGUI finalText;
    public DialogueText startText;
    public DialogueText endText;
    public TextMeshProUGUI tipText;

    private float timeBeforePause;
    [Header("Pause Menu")]
    public GameObject mainScreen;
    public GameObject pauseScreen;
    public Transform armors;
    public Transform slots;

    public Image dashImage;
    public Image doubleJumpImage;
    public Image finalItemImage;
    public Image keyImage;
    public Image mineralImage;

    [Header("Music")]
    public AudioSource music;
    public AudioClip mainMusic;
    public AudioClip fightMusic;

    private void Start()
    {
        MainManager.Instance.LoadPlayer();

        player = GameObject.FindWithTag("Player");
        moneyText.text = MainManager.Instance.money.ToString();

        startedGame = MainManager.Instance.hasStarted;
        if(!startedGame)
            StartGame();
        else
        {
            SetLoadedContent();
        }
    }

    private void Update()
    {
        if (MainManager.Instance.hasMap && startedGame && !endedGame && Input.GetKeyDown(KeyCode.M) && !MainManager.Instance.shopping)
            ToggleMap();

        if (startedGame && !endedGame && Input.GetKeyDown(KeyCode.Escape) && !MainManager.Instance.shopping)
            TogglePause();

        if (Input.GetKeyDown(KeyCode.Escape) && MainManager.Instance.shopping)
        {
            CloseShop(false);
        }

        if(mapScreen.activeSelf)
        {
            playerImage.transform.localPosition = player.transform.position * 2.5f;
        }
    }

    public void SetLoadedContent()
    {
        FillPauseMenu();

        endBoxDialogue.SetActive(MainManager.Instance.obtainedFinalItem);
        finalInteract.SetActive(MainManager.Instance.obtainedFinalItem);

        HealStation[] stations = FindObjectsByType<HealStation>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for(int i = 0; i < stations.Length; i++)
        {
            if(stations[i].stationName.ToLower() == MainManager.Instance.currentStation)
            {
                player.GetComponent<PlayerHealth>().lastStation = stations[i].gameObject;
                break;
            }
        }

        player.transform.position = player.GetComponent<PlayerHealth>().lastStation.transform.position;
        RoomManager currentRoom = player.GetComponent<PlayerHealth>().lastStation.GetComponentInParent<RoomManager>();

        if(currentRoom != MainManager.Instance.currentRoom)
            MainManager.Instance.currentRoom.LeaveRoom();

        currentRoom.EnterRoom();
        startDoor.EndMovement();
        PlayMainMusic();
    }

    public void OpenScene(string sceneName)
    {
        Time.timeScale = 1;
        MainManager.Instance.SavePlayer();
        SceneManager.LoadScene(sceneName);
    }

    public void StopMusic()
    {
        music.Stop();
    }

    public void PlayFightMusic()
    {
        music.Stop();
        music.clip = fightMusic;
        music.Play();
    }

    public void PlayMainMusic()
    {
        music.Stop();
        music.clip = mainMusic;
        music.Play();
    }

    public void TogglePause()
    {
        paused = !paused;

        mainScreen.SetActive(!paused);
        pauseScreen.SetActive(paused);
        Cursor.visible = paused;

        if (paused)
        {
            timeBeforePause = Time.timeScale;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;

            FillPauseMenu();
        }
        else
        {
            Time.timeScale = timeBeforePause;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void FillPauseMenu()
    {
        FillArmors();
        FillSlots();
        FillImages();
    }

    private void FillArmors()
    {
        int extraHearts = MainManager.Instance.hpPieces / 3;
        int count = 0;
        for (int i = 0; i < armors.childCount; i++)
        {
            for (int j = 0; j < armors.GetChild(i).childCount; j++)
            {
                if (MainManager.Instance.hpPieces > count)
                    armors.GetChild(i).GetChild(j).gameObject.GetComponent<Image>().color = Color.white;
                else
                    armors.GetChild(i).GetChild(j).gameObject.GetComponent<Image>().color = Color.black;

                count++;
            }

            if (extraHearts > i)
                armors.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
            else
                armors.GetChild(i).gameObject.GetComponent<Image>().color = Color.black;
        }
    }

    private void FillSlots()
    {
        for (int i = 0; i < slots.childCount; i++)
        {
            if (MainManager.Instance.slots > i)
                slots.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
            else
                slots.GetChild(i).gameObject.GetComponent<Image>().color = Color.black;
        }
    }

    private void FillImages()
    {
        dashImage.gameObject.SetActive(MainManager.Instance.hasDash);
        doubleJumpImage.gameObject.SetActive(MainManager.Instance.hasDoubleJump);
        finalItemImage.gameObject.SetActive(MainManager.Instance.obtainedFinalItem);

        keyImage.gameObject.SetActive(MainManager.Instance.keys > 0);
        mineralImage.gameObject.SetActive(MainManager.Instance.minerals > 0);
        keyImage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = MainManager.Instance.keys.ToString();
        mineralImage.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = MainManager.Instance.minerals.ToString();
    }

    private void StartGame()
    {
        StartCoroutine(StartMessage());
    }

    private IEnumerator StartMessage()
    {
        StopPlayer();
        fade.color = new Color32(0, 0, 0, 255);

        yield return new WaitForSeconds(1);

        DisplayNextLine(startText, null, finalText, true);
    }

    private IEnumerator StartGameCoroutine()
    {
        finalText.text = "";

        yield return new WaitForSeconds(1);

        for (int i = 255; i > 0; i -= 15)
        {
            fade.color = new Color32(0, 0, 0, (byte)i);
            yield return new WaitForSeconds(0.015f);
        }
        fade.color = new Color32(0, 0, 0, 0);

        yield return new WaitForSeconds(0.75f);

        startedGame = true;
        MainManager.Instance.hasStarted = true;
        player.GetComponent<PlayerMovement>().StartTransition(RoomTransition.Direction.Right);
        yield return new WaitForSeconds(1);
        player.GetComponent<PlayerMovement>().EndTransition();
        StopPlayer();
        yield return new WaitForSeconds(0.5f);
        startDoor.TriggerDoor();
        yield return new WaitForSeconds(1);
        StartPlayer();

        yield return new WaitForSeconds(2);
        PlayMainMusic();
        bossText.text = "The Outside";
        bossText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        bossText.gameObject.SetActive(false);

        yield return new WaitForSeconds(2);
        ShowTip("Press Z or LMB to attack");
    }

    public void ShowTip(string text)
    {
        StartCoroutine(Tip(text));
    }

    private IEnumerator Tip(string text)
    {
        tipText.text = text;
        tipText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        tipText.gameObject.SetActive(false);
    }

    private IEnumerator EnterBox()
    {
        MainManager.Instance.SavePlayer();
        endedGame = true;
        player.transform.eulerAngles = new Vector3(0, 180, 0);
        StopPlayer();

        yield return new WaitForSeconds(1);
        startDoor.TriggerDoor();

        yield return new WaitForSeconds(1.5f);
        player.GetComponent<PlayerMovement>().StartTransition(RoomTransition.Direction.Left);
        yield return new WaitForSeconds(2);
        player.GetComponent<PlayerMovement>().EndTransition();
        StopPlayer();
        startDoor.TriggerDoor();

        yield return new WaitForSeconds(1);

        for (int i = 0; i < 255; i += 15)
        {
            fade.color = new Color32(0, 0, 0, (byte)i);
            yield return new WaitForSeconds(0.015f);
        }
        fade.color = new Color32(0, 0, 0, 255);

        yield return new WaitForSeconds(1.3f);

        DisplayNextLine(endText, null, finalText, true);
    }

    public void Heal()
    {
        healFactor.Play("Heal");
    }

    public void ToggleMap()
    {
        if(!mapScreen.activeSelf)
        {
            playerImage.transform.localPosition = player.transform.position * 2.5f;

            if (MainManager.Instance.obtainedFinalItem)
                objectiveImage.transform.localPosition = finalInteract.transform.position * 2.5f;
            else
                objectiveImage.transform.localPosition = finalItem.transform.position * 2.5f;

            foreach (Transform t in mapParent)
                Destroy(t.gameObject);

            RoomManager[] rooms = FindObjectsByType<RoomManager>(FindObjectsSortMode.None);
            List<RoomTransition> existingTrans = new List<RoomTransition>();
            foreach(RoomManager room in rooms)
            {
                if(room.visited)
                {
                    GameObject coll = room.camCollider;
                    GameObject newRoom = Instantiate(mapRoom, transform.position, transform.rotation);
                    newRoom.transform.SetParent(mapParent, false);
                    newRoom.transform.localScale = coll.transform.localScale * 2.5f;
                    newRoom.transform.localPosition = coll.transform.position * 2.5f;
                    newRoom.name = room.gameObject.name;

                    if(room.gameObject.name.Contains("Box"))
                        newRoom.GetComponent<Image>().color = Color.yellow;
                                     
                    RoomTransition[] transitions = room.gameObject.GetComponentsInChildren<RoomTransition>();

                    foreach(RoomTransition t in transitions)
                    {
                        if (!existingTrans.Contains(t.room.gameObject.GetComponent<RoomTransition>()))
                        {
                            RoomTransition.Direction direction = t.direction;

                            float xScale = 0, yScale = 0;

                            if (t.direction == RoomTransition.Direction.Right || t.direction == RoomTransition.Direction.Left)
                            {
                                yScale = t.transform.localScale.y + 1;
                                xScale = Mathf.Abs(t.transform.position.x - t.room.gameObject.transform.position.x) + 10;
                            }

                            if (t.direction == RoomTransition.Direction.Up || t.direction == RoomTransition.Direction.Down)
                            {
                                xScale = t.transform.localScale.x + 1;
                                yScale = Mathf.Abs(t.transform.position.y - t.room.gameObject.transform.position.y) + 10;
                            }

                            GameObject newTrans = Instantiate(mapRoom, transform.position, transform.rotation);
                            newTrans.transform.SetParent(mapParent, false);
                            newTrans.transform.localScale = new Vector3(xScale, yScale, 1) * 2.5f;
                            newTrans.transform.localPosition = ((t.transform.position + t.room.gameObject.transform.position) / 2) * 2.5f;
                            existingTrans.Add(t);
                        }
                    }
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
        player.GetComponent<PlayerMovement>().StopMovement();
    }

    private void StartPlayer()
    {
        player.GetComponent<PlayerMovement>().StartMovement();
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
        shopConfirmPriceIcon.gameObject.SetActive(true);
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
        MainManager.Instance.boughtShopItems = item.merchant.boughtItems.ToArray();

        ObtainObject(data.itemName, data.icon);

        CloseShop(item.merchant.boughtItems.Count == item.merchant.items.Length);
    }

    public void GetMoney(int amount)
    {
        if (amount > 0)
            GameObject.Find("CoinSound").GetComponent<AudioSource>().Play();

        MainManager.Instance.money += amount;
        moneyText.text = MainManager.Instance.money.ToString();
    }

    public void DisplayNextLine(DialogueText dialogueText, NPCDialogue npc, TextMeshProUGUI textBox, bool automatic)
    {
        currentDialogueText = dialogueText;
        currentNPC = npc;
        if(lines.Count == 0)
        {
            if (!dialogueEnded)
                StartDialogue(dialogueText, textBox == this.dialogueText);
            else if (dialogueEnded && !typing)
            {
                EndDialogue();
                return;
            }
        }

        if(!typing)
        {
            line = lines.Dequeue();
            dialogueCoroutine = StartCoroutine(TypeDialogueText(line, textBox, automatic));
        }else
        {
            FinishLineEarly();
        }

        if (lines.Count == 0)
        {
            dialogueEnded = true;
        }
    }

    private void StartDialogue(DialogueText dialogueText, bool dialogue)
    {
        StopPlayer();

        dialogueBox.SetActive(dialogue);
        dialogueName.text = dialogueText.speakerName;

        for (int i = 0; i < dialogueText.lines.Length; i++)
        {
            lines.Enqueue(dialogueText.lines[i]);
        }
    }

    private void EndDialogue()
    {
        if(startedGame && !endedGame)
        {
            player.GetComponent<PlayerAttack>().canAttack = true;
            player.GetComponent<PlayerMovement>().canMove = true;
        }

        lines.Clear();
        dialogueEnded = false;
        dialogueBox.SetActive(false);

        if(currentNPC != null)
            currentNPC.OnDialogueEnd();

        if(!startedGame)
        {
            StartCoroutine(StartGameCoroutine());
        }

        if(endedGame)
        {
            StartCoroutine(OpenCredits());
        }
    }

    private IEnumerator OpenCredits()
    {
        yield return new WaitForSeconds(3);
        OpenScene("Credits");
    }

    private IEnumerator TypeDialogueText(string line, TextMeshProUGUI text, bool automatic)
    {
        typing = true;

        text.text = "";
        string originalText = line;
        string displayText = "";
        int alpha = 0;

        foreach (char c in line.ToCharArray())
        {
            alpha++;
            text.text = originalText;

            displayText = text.text.Insert(alpha, "<color=#00000000>");
            text.text = displayText;

            yield return new WaitForSeconds(0.03f);
        }

        typing = false;

        if(automatic)
        {
            yield return new WaitForSeconds(1.25f);
            DisplayNextLine(currentDialogueText, currentNPC, text, automatic);
        }
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
        GetComponent<AudioSource>().Play();

        switch(objName)
        {
            case "dash": 
                player.GetComponent<PlayerMovement>().hasDash = true;
                MainManager.Instance.hasDash = true;

                obtain = "Obtained";
                upName = "Phase Shifter";
                upDesc = "Increases your movement options.\nPress C to dash forward.";

                DisplayUpgrade(obtain, upName, upDesc, image);
                break;
            case "doubleJump":
                player.GetComponent<PlayerMovement>().hasDoubleJump = true;
                MainManager.Instance.hasDoubleJump = true;

                obtain = "Obtained";
                upName = "Boot Module";
                upDesc = "Increases your maximum height.\nPress SPACE in the air to double jump.";

                DisplayUpgrade(obtain, upName, upDesc, image);
                break;
            case "map":
                MainManager.Instance.hasMap = true;

                obtain = "Obtained";
                upName = "Map";
                upDesc = "A better look at The Outside.\nPress M to open the map and see your location.";
                DisplayUpgrade(obtain, upName, upDesc, image);
                break;
            case "finalitem":
                MainManager.Instance.obtainedFinalItem = true;

                obtain = "Obtained";
                upName = "The Item";
                upDesc = "The item that The Box has been looking for.\nThey would be pleased to have this.";

                endBoxDialogue.SetActive(true);
                finalInteract.SetActive(true);
                DisplayUpgrade(obtain, upName, upDesc, image);
                break;
            case "battery":
                MainManager.Instance.slots++;
                player.GetComponent<PlayerAttack>().IncreaseMaxHealCount();

                DisplayObject("Battery Slot", image);
                break;
            case "key":
                MainManager.Instance.keys++;
                DisplayObject("Key", image);
                break;
            case "mineral":
                MainManager.Instance.minerals++;
                DisplayObject("Mineral", image);
                break;
            case "armorPiece":
                StartCoroutine(ObtainHealthPiece());
                DisplayObject("Amour Piece", image);
                break;
            case "endGame":
                StartCoroutine(EnterBox());
                break;
        }

        MainManager.Instance.SavePlayer();
    }

    public void DisplayObject(string objName, Sprite image)
    {
        obtainObjectText.text = objName;
        obtainObjectImage.sprite = image;

        obtainObject.SetActive(true);

        StartCoroutine(CloseItem());
    }

    private IEnumerator CloseItem()
    {
        yield return new WaitForSeconds(2);
        obtainObject.SetActive(false);
    }

    public void DisplayUpgrade(string obtain, string upName, string upDesc, Sprite image)
    {
        StopPlayer();

        obtainText.text = obtain;
        upgradeNameText.text = upName;
        upgradeDescText.text = upDesc;
        upgradeImage.sprite = image;

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
        shopConfirmIcon.sprite = mineralImage.sprite;
        shopConfirmPriceIcon.gameObject.SetActive(false);
        shopConfirmButton.Select();

        shopConfirmButton.onClick.RemoveAllListeners();
        shopConfirmButton.onClick.AddListener(delegate { UpgradeWeapon(station); });

        shopDeclineButton.onClick.RemoveAllListeners();
        shopDeclineButton.onClick.AddListener(delegate { CancelUpgrade(); });

        shopConfirm.SetActive(true);
    }

    public void UseKey(HealStation station)
    {
        StopPlayer();

        shopConfirmText.text = "Use Key?";
        shopConfirmPrice.text = "1";
        shopConfirmIcon.sprite = keyImage.sprite;
        shopConfirmPriceIcon.gameObject.SetActive(false);
        shopConfirmButton.Select();

        shopConfirmButton.onClick.RemoveAllListeners();
        shopConfirmButton.onClick.AddListener(delegate { OpenDoor(station); });

        shopDeclineButton.onClick.RemoveAllListeners();
        shopDeclineButton.onClick.AddListener(delegate { CancelUpgrade(); });

        shopConfirm.SetActive(true);
    }

    public void SaveStation(HealStation station)
    {
        for(int i = 0; i < MainManager.Instance.stations.Length; i++)
        {
            if(station.stationName.ToLower() == MainManager.Instance.stations[i].stationName.ToLower())
            {
                MainManager.Instance.stations[i].used = station.used;
            }
        }

        MainManager.Instance.SavePlayer();
    }

    public void OpenDoor(HealStation station)
    {
        if (MainManager.Instance.keys > 0)
        {
            station.used = true;
            MainManager.Instance.keys--;
            shopConfirm.SetActive(false);
            player.GetComponent<PlayerAttack>().canAttack = true;
            player.GetComponent<PlayerMovement>().canMove = true;

            station.door.TriggerDoor();
            SaveStation(station);
        }
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

            DisplayObject("Weapon Upgraded", upgradeIcon);
            SaveStation(station);
        }
    }

    public IEnumerator ObtainHealthPiece()
    {
        DisplayHealthPieces();
        healthPieces.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.25f);
        MainManager.Instance.hpPieces++;

        if (MainManager.Instance.hpPieces % 3 == 0)
        {
            for (int i = 0; i < healthPieces.childCount; i++)
                healthPieces.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;

            yield return new WaitForSeconds(1.25f);

            player.GetComponent<PlayerHealth>().IncreaseHp();
            healthPieces.gameObject.SetActive(false);
        }
        else
            DisplayHealthPieces();

        yield return new WaitForSeconds(1.25f);
        healthPieces.gameObject.SetActive(false);
    }


    public void DisplayHealthPieces()
    {
        int r = MainManager.Instance.hpPieces % 3;

        for(int i = 0; i < healthPieces.childCount; i++)
        {
            if (r > i)
                healthPieces.GetChild(i).gameObject.GetComponent<Image>().color = Color.white;
            else
                healthPieces.GetChild(i).gameObject.GetComponent<Image>().color = Color.black;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
