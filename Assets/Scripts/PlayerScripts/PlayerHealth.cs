using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public Transform healthParent;
    public GameObject healthIcon;

    public GameObject lastStation;

    public bool isHealing;
    private bool immune;
    public bool canHeal;
    public bool respawning;

    public Vector3 spawnPoint;
    public Vector3 respawnPoint;

    public Material flash;
    private Material original;
    private SpriteRenderer spriteRenderer;

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    public Image transitionFade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        original = spriteRenderer.material;

        health = maxHealth;
        spawnPoint = transform.position;
        immune = false;
        isHealing = false;
        respawning = false;

        if (gameObject.CompareTag("Player"))
        {
            playerAttack = GetComponent<PlayerAttack>();
            playerMovement = GetComponent<PlayerMovement>();

            for (int i = 0; i < health; i++)
            {
                GameObject newIcon = Instantiate(healthIcon, healthParent.transform.position, healthIcon.transform.rotation);
                newIcon.transform.SetParent(healthParent.transform, false);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.CompareTag("Player") && Input.GetButtonDown("Fire2"))
        {
            if(playerAttack.currentHeal >= 4 && health < maxHealth && !isHealing)
            {
                playerAttack.currentHeal -= 4;
                playerAttack.UpdateHealIcons();
                playerMovement.StartCoroutine(playerMovement.Heal());
                isHealing = true;
            }
        }
    }

    public void TakeDamage() { TakeDamage(1); }

    public void TakeDamage(int damage)
    {
        if (!immune)
        {
            health -= damage;

            if (health <= 0)
            {
                if (gameObject.CompareTag("Enemy"))
                {
                    if (GetComponent<Boss>() == null)
                    {
                        GetComponent<Enemy>().GiveMoney();
                        gameObject.SetActive(false);
                    }
                    else
                        GetComponent<Boss>().EndFight();
                }
                else if(!respawning)
                    Die();
            }

            if (gameObject.CompareTag("Player"))
            {
                healthParent.GetChild(health).GetComponent<Image>().color = Color.black;
                if(health > 0)
                    StartCoroutine(Immunity());
            }else if(gameObject.activeSelf)
                StartCoroutine(Flash());
        }        
    }

    public IEnumerator Immunity()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        immune = true;
        float alpha = 0;
        for(int i = 0; i < 10; i++)
        {
            renderer.color = new Color32(255, 255, 255, (byte)alpha);
            if (alpha == 0)
                alpha += 255;
            else
                alpha -= 255;
            yield return new WaitForSeconds(0.1f);
        }
        immune = false;
    }

    public void Heal()
    {
        health++;
        health = Mathf.Clamp(health, 0, maxHealth);

        if(health <= healthParent.childCount)
            healthParent.GetChild(health-1).GetComponent<Image>().color = Color.white;
    }

    public void Die()
    {
        FindAnyObjectByType<GameManager>().StopMusic();
        GetComponent<Animator>().Play("Dead");
        StartCoroutine(DieTransition());
    }

    private IEnumerator DieTransition()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        
        immune = true;
        respawning = true;
        playerAttack.canAttack = false;

        yield return new WaitForSeconds(1);
        for (int i = 0; i < 255; i += 15)
        {
            transitionFade.color = new Color32(0, 0, 0, (byte)i);
            yield return new WaitForSeconds(0.025f);
        }
        transitionFade.color = new Color32(0, 0, 0, 255);

        Respawn();
        yield return new WaitForSeconds(2);

        transitionFade.gameObject.transform.GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        transitionFade.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(0.75f);

        for (int i = 255; i > 0; i -= 15)
        {
            transitionFade.color = new Color32(0, 0, 0, (byte)i);
            yield return new WaitForSeconds(0.025f);
        }
        transitionFade.color = new Color32(0, 0, 0, 0);

        yield return new WaitForSeconds(0.75f);

        respawning = false;
        immune = false;
        playerAttack.canAttack = true;

        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Respawn()
    {
        MainManager.Instance.currentRoom.LeaveRoom();
        lastStation.GetComponentInParent<RoomManager>().EnterRoom();
        for (int i = health; i < maxHealth; i++)
            Heal();

        FindAnyObjectByType<GameManager>().PlayMainMusic();
        transform.position = lastStation.transform.position;
        GetComponent<Animator>().Play("Idle");
    }

    public IEnumerator Flash()
    {
        spriteRenderer.material = flash;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material = original;
    }

    public void IncreaseHp()
    {
        IncreaseHp(1);
    }

    public void IncreaseHp(int amount)
    {
        maxHealth += amount;
        health = maxHealth;

        foreach(Transform c in healthParent.transform)
            c.gameObject.GetComponent<Image>().color = Color.white;

        for (int i = 0; i < amount; i++)
        {
            GameObject newIcon = Instantiate(healthIcon, healthParent.transform.position, healthIcon.transform.rotation);
            newIcon.transform.SetParent(healthParent.transform, false);
        }
    }

    public void ResetPosition()
    {
        if(health > 0)
        {
            transform.position = respawnPoint;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}
