using System.Collections;
using UnityEngine;

public class HealStation : MonoBehaviour
{
    public string stationName;
    public bool playerIn;
    private PlayerHealth health;
    public GameObject recoverText;
    public Door door;

    public enum StationType { Heal, Upgrade, Key}
    public StationType type;
    public bool used;

    private void Start()
    {
        for (int i = 0; i < MainManager.Instance.stations.Length; i++)
        {
            if (stationName.ToLower() == MainManager.Instance.stations[i].stationName.ToLower())
            {
                used = MainManager.Instance.stations[i].used;
                break;
            }
        }

        if(used && type == StationType.Key)
        {
            door.gameObject.transform.position = door.transform.position - new Vector3(0, door.distance, 0);
            door.speed = -door.speed;
        }

        recoverText.SetActive(false);
    }

    private void Update()
    {
        if(playerIn && Input.GetButtonDown("Vertical"))
        {
            if(type == StationType.Heal)
                Heal();

            if (type == StationType.Upgrade && !used)
                FindAnyObjectByType<GameManager>().UpgradeWeaponPopUp(this);

            if (type == StationType.Key && !used)
                FindAnyObjectByType<GameManager>().UseKey(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !used)
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
        MainManager.Instance.currentStation = stationName;
        MainManager.Instance.SavePlayer();
        GetComponent<AudioSource>().Play();

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
