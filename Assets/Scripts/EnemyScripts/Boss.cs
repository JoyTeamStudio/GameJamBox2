using System.Collections;
using TMPro;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public PlayerHealth health;
    public bool hasStarted;
    public bool hasEnded;
    [HideInInspector] public GameObject player;
    public Rigidbody2D rb;

    public Door[] doors;
    public string bossName;
    public string bossDisplayName;
    public TextMeshProUGUI bossNameText;

    public Vector3 startPos;
    public Animator animator;
    public GameObject deadCorpse;

    public float startDelay;
    public float timeForParticles;
    public float timeForCorpse;

    private void Start()
    {
        startPos = transform.position;
        animator = GetComponent<Animator>();
    }

    public void ResetBoss()
    {
        hasStarted = false;
        transform.position = startPos;
        transform.eulerAngles = Vector3.zero;
        ((MonoBehaviour)GetComponent(bossName)).StopAllCoroutines();
        animator.Play("Idle");

        if (bossName == "dirtGuardian")
        {
            GetComponent<DirtGuardian>().isMoving = false;
            transform.localScale = new Vector3(4, 4, 1);
        }

        foreach (Door d in doors)
        {
            d.TriggerDoor();
        }

        health.health = health.maxHealth;
    }

    public void StartBoss()
    {
        hasStarted = true;
        hasEnded = false;
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();

        foreach (Door d in doors)
            d.TriggerDoor();

        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        if (bossName == "DirtGuardian")
            animator.Play("Awake");

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        movement.StopMovement();
        yield return new WaitForSeconds(startDelay);
        bossNameText.text = bossDisplayName;
        bossNameText.gameObject.SetActive(true);

        animator.Play("Scream");

        yield return new WaitForSeconds(3);
        movement.StartMovement();
        bossNameText.gameObject.SetActive(false);

        if (bossName == "FirstBoss")
            GetComponent<FirstBoss>().Attack();

        if (bossName == "DirtGuardian")
            GetComponent<DirtGuardian>().StartFight();
    }

    public void EndFight()
    {
        if(!hasEnded)
            StartCoroutine(EndFightCoroutine());
    }

    private IEnumerator EndFightCoroutine()
    {
        hasEnded = true;
        MainManager.Instance.AddBoss(this);
        ((MonoBehaviour)GetComponent(bossName)).StopAllCoroutines();
        GetComponent<Collider2D>().enabled = false;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        Time.timeScale = 0.25f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;

        yield return new WaitForSeconds(timeForParticles);
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(timeForCorpse);
        Instantiate(deadCorpse, transform.position, transform.rotation);
        GetComponent<SpriteRenderer>().enabled = false;

        foreach (Transform t in transform)
            t.gameObject.SetActive(false);

        yield return new WaitForSeconds(2);

        foreach (Door d in doors)
            d.TriggerDoor();
    }
}
