using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class NPCDialogue : MonoBehaviour, IDialogue
{
    public string dialogueName;

    public DialogueText[] dialogue;
    public int dialogueIndex;
    public bool playerIn;
    public GameObject talkText;

    public bool talkOnTrigger;
    private GameObject player;
    private GameManager gameManager;

    public Transform dialogueFocus;

    private void Start()
    {
        talkText.SetActive(false);
        player = GameObject.FindWithTag("Player");
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
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
            case "littlehermit":
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
    }
}
