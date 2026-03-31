using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ShopMerchant : MonoBehaviour
{
    public ShopItemData[] items;
    public List<int> boughtItems;

    public bool playerIn;
    public GameObject shopText;

    private GameObject player;
    private GameManager gameManager;

    private void Start()
    {
        shopText.SetActive(false);
        player = GameObject.FindWithTag("Player");
        gameManager = FindAnyObjectByType<GameManager>();

        boughtItems = MainManager.Instance.boughtShopItems.OfType<int>().ToList();
    }

    private void Update()
    {
        if (playerIn && Input.GetButtonDown("Vertical") && !MainManager.Instance.shopping && GetComponent<NPCDialogue>() == null)
        {
            OpenShop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIn = true;
            shopText.SetActive(playerIn);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIn = false;
            shopText.SetActive(playerIn);
        }
    }

    public void OpenShop()
    {
        if (boughtItems.Count < items.Length)
            gameManager.OpenShop(items, boughtItems, this);
    }
}
