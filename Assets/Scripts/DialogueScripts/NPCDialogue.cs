using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class NPCDialogue : MonoBehaviour, IDialogue
{
    private SpriteRenderer spriteRenderer;
    public string dialogueName;

    public DialogueText[] dialogue;
    public int dialogueIndex;
    public bool playerIn;
    public GameObject talkText;

    public bool talkOnTrigger;
    private GameObject player;
    private GameManager gameManager;

    public Transform dialogueFocus;
    public bool flip;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        talkText.SetActive(false);
        player = GameObject.FindWithTag("Player");
        gameManager = FindAnyObjectByType<GameManager>();

        GetDialogueIndex();
    }

    private void GetDialogueIndex()
    {
        for(int i = 0; i < MainManager.Instance.npcData.Length; i++)
        {
            if(dialogueName.ToLower() == MainManager.Instance.npcData[i].npcName.ToLower())
            {
                dialogueIndex = MainManager.Instance.npcData[i].dialogueIndex;
                break;
            }    
        }
    }

    private void Update()
    {
        if(flip)
            spriteRenderer.flipX = player.transform.position.x < transform.position.x;

        if (playerIn && Input.GetButtonDown("Vertical"))
        {
            OpenDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIn = true;

            if (talkOnTrigger)
                OpenDialogue();
            else
                talkText.SetActive(playerIn);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIn = false;
            talkText.SetActive(playerIn);
        }
    }

    public void OpenDialogue()
    {
        if(dialogueIndex >= 0 && !MainManager.Instance.shopping)
        {
            if (dialogueFocus != null)
                MainManager.Instance.FocusCamera(dialogueFocus);
            gameManager.DisplayNextLine(dialogue[dialogueIndex], this, gameManager.dialogueText, false);
        }
    }

    public void OnDialogueEnd()
    {
        if (dialogueFocus != null)
            MainManager.Instance.FocusCamera(player.transform);

        switch (dialogueName)
        {
            case "shopGreet" or "endBox": dialogueIndex = -1;
                break;
            case "shopkeeper":
                Debug.Log("shopkeeper");
                if(dialogueIndex == 0 || dialogueIndex == 2)
                {
                    GetComponent<ShopMerchant>().OpenShop();
                }

                if (dialogueIndex == 1)
                    dialogueIndex = 2;

                if (dialogueIndex == 3)
                    dialogueIndex = 4;

                break;
            case "littlehermit1" or "littlehermit2":
                if(dialogueIndex == 0)
                    dialogueIndex = 1;
                break;
            case "explorer":
                if(dialogueIndex == 0)
                {
                    gameManager.ObtainObject("map", null);
                    dialogueIndex = 1;
                }
                break;
        }

        UpdateDialogueIndex();
    }

    public void OnShopEnd()
    {
        switch(dialogueName)
        {
            case "shopkeeper":
                if (dialogueIndex == 0)
                {
                    dialogueIndex = 1;
                    OpenDialogue();
                }
                break;
        }

        UpdateDialogueIndex();
    }

    public void OnEmptyShop()
    {
        switch (dialogueName)
        {
            case "shopkeeper":
                dialogueIndex = 3;
                OpenDialogue();
                break;
        }

        UpdateDialogueIndex();
    }

    private void UpdateDialogueIndex()
    {
        for (int i = 0; i < MainManager.Instance.npcData.Length; i++)
        {
            if (dialogueName.ToLower() == MainManager.Instance.npcData[i].npcName.ToLower())
            {
                MainManager.Instance.npcData[i].dialogueIndex = dialogueIndex;
                break;
            }
        }
    }
}
