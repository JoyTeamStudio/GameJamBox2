using System.Collections;
using UnityEngine;

public class HealStation : MonoBehaviour
{
    public bool playerIn;
    private PlayerHealth health;
    public GameObject recoverText;

    private void Start()
    {
        recoverText.SetActive(false);
    }

    private void Update()
    {
        if(playerIn && Input.GetButtonDown("Vertical"))
        {
            Heal();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            health = collision.gameObject.GetComponent<PlayerHealth>();
            playerIn = true;
            recoverText.SetActive(playerIn);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIn = false;
            recoverText.SetActive(playerIn);
        }
    }

    public void Heal()
    {
        StartCoroutine(HealPlayer());

        FindAnyObjectByType<GameManager>().Heal();
        RoomManager[] rooms = FindObjectsByType<RoomManager>(FindObjectsSortMode.None);
        foreach(RoomManager room in rooms)
            room.visitedSinceLastHeal = false;
    }

    private IEnumerator HealPlayer()
    {
        health.spawnPoint = transform.position;
        health.lastStation = gameObject;

        for (int i = health.health; i < health.maxHealth; i++)
        {
            health.Heal();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
