using System.Collections;
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

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    public void ResetBoss()
    {
        hasStarted = false;
        transform.position = startPos;
        transform.eulerAngles = Vector3.zero;
        ((MonoBehaviour)GetComponent(bossName)).StopAllCoroutines();

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
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.3f);
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        movement.StopMovement();
        yield return new WaitForSeconds(2);
        movement.StartMovement();

        if(bossName == "FirstBoss")
            GetComponent<FirstBoss>().Attack();
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
        Destroy(rb);
        Time.timeScale = 0.25f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;

        yield return new WaitForSeconds(2);

        foreach (Door d in doors)
            d.TriggerDoor();
    }
}
