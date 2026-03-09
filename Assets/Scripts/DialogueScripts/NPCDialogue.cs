using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public DialogueText dialogue;
    public bool playerIn;
    public GameObject talkText;

    private GameObject player;
    private GameManager gameManager;

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
            gameManager.DisplayNextLine(dialogue);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIn = true;
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
}
