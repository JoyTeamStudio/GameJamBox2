using System.Collections;
using UnityEngine;

public class DirtGuardian : MonoBehaviour
{
    private Boss bossManager;

    public bool isMoving;
    private Vector3 moveDir;
    public Vector3 target;
    public float speed;
    public float diveSpeed = 5f;
    public float moveSpeed = 10f;
    public float dashSpeed = 10f;

    private int dashCount;
    private int lastAttack;
    public string currentAttack;

    public GameObject particles;
    public float upParticleSpawn;
    public float downParticleSpawn;

    public GameObject projectile;
    private bool dead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossManager = GetComponent<Boss>();
    }

    private void Update()
    {
        if(isMoving)
        {
            transform.Translate(speed * Time.deltaTime * moveDir);

            if(Vector3.Distance(transform.position, target) < 0.5f)
            {
                transform.position = target;
                isMoving = false;

                switch (currentAttack)
                {
                    case "diveUp":
                        if(!dead)
                            StartCoroutine(MoveAttack());
                        break;
                    case "diveDown":
                        if (bossManager.health.health <= 0)
                            StartCoroutine(Death());
                        else
                            Attack();
                        break;
                    case "move":
                        if(!dead)
                            StartCoroutine(Dive());
                        break;
                    case "dashUpAndDown":
                        if (bossManager.health.health <= 0)
                        {
                            StartCoroutine(Death());
                        }
                        else
                        {
                            if (dashCount > 0)
                                StartCoroutine(DashUpAndDown());
                            else
                                Attack();
                        }
                        break;
                }
                
            }
        }

        if (bossManager.health.health <= 0 && !dead && currentAttack != "dashUpAndDown")
        {
            isMoving = false;
            dead = true;
            StopAllCoroutines();

            switch (currentAttack)
            {
                case "wall":
                    StartCoroutine(DeformWall());
                    break;
                default:
                    StartCoroutine(Dive());
                    break;
            }
        }
    }

    public void StartFight()
    {
        dead = false;
        lastAttack = -1;
        GetComponent<Collider2D>().enabled = true;
        StartCoroutine(Dive());
    }

    public IEnumerator Dive()
    {
        bossManager.animator.Play("Dive");
        yield return new WaitForSeconds(0.35f);

        isMoving = true;
        target = transform.position - new Vector3(0, 5, 0);
        moveDir = Vector3.down;
        speed = diveSpeed;
        currentAttack = "diveDown";

        bossManager.rb.gravityScale = 0;
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Attack()
    {
        int random;
        do
        {
            random = Random.Range(0, 3);
        } while (random == lastAttack);

        lastAttack = random;

        switch(random)
        {
            case 0:
                StartCoroutine(DiveUp());
                break;
            case 1:
                dashCount = 8;
                StartCoroutine(DashUpAndDown());
                break;
            case 2:
                StartCoroutine(WallAttack());
                break;
        }
    }

    public IEnumerator DiveUp()
    {
        bossManager.animator.Play("Dive");
        int random = Random.Range(0, 2);
        int mult = 1;
        transform.eulerAngles = Vector3.zero;

        if (random == 0)
        {
            mult = -1;
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        transform.position = new Vector3(bossManager.startPos.x + (14 * mult), transform.position.y, transform.position.z);
        GameObject newParticles = Instantiate(particles, transform.position, transform.rotation);
        newParticles.transform.position = new Vector3(transform.position.x, bossManager.startPos.y + downParticleSpawn, transform.position.z);
        Destroy(newParticles, 0.75f);

        yield return new WaitForSeconds(0.75f);

        isMoving = true;
        target = transform.position + new Vector3(0, 5, 0);
        moveDir = Vector3.up;
        speed = diveSpeed;
        currentAttack = "diveUp";
    }

    public IEnumerator DashUpAndDown()
    {
        dashCount--;
        bossManager.animator.Play("Dash");
        float random = Random.Range(bossManager.startPos.x - 14, bossManager.startPos.x + 14);
        transform.position = new Vector3(random, transform.position.y, transform.position.z);
        moveDir = Vector3.up;

        float speedMult = 1;
        float particleTime = 0.75f;

        if (bossManager.health.health < bossManager.health.maxHealth / 2)
        {
            speedMult = 1.4f;
            particleTime = 0.5f;
        }

        GameObject newParticles = Instantiate(particles, transform.position, transform.rotation);
        Destroy(newParticles, particleTime);

        if (transform.position.y < bossManager.startPos.y)
        {
            transform.eulerAngles = Vector3.zero;
            target = new Vector3(transform.position.x, transform.position.y + 15, transform.position.z);
            newParticles.transform.position = new Vector3(transform.position.x, bossManager.startPos.y + downParticleSpawn, transform.position.z);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 180);
            target = new Vector3(transform.position.x, transform.position.y - 15, transform.position.z);
            newParticles.transform.position = new Vector3(transform.position.x, bossManager.startPos.y + upParticleSpawn, transform.position.z);
            newParticles.GetComponent<ParticleSystem>().gravityModifier = 1;
        }

        yield return new WaitForSeconds(particleTime);

        speed = dashSpeed * speedMult;
        isMoving = true;
        currentAttack = "dashUpAndDown";
    }

    public IEnumerator WallAttack()
    {
        yield return new WaitForSeconds(0.75f);
        currentAttack = "wall";
        bossManager.animator.Play("FormWall");
        GetComponent<Collider2D>().enabled = false;
        int random = Random.Range(0, 2);
        int mult = -1;
        transform.eulerAngles = Vector3.zero;

        if (random == 0)
        {
            mult = 1;
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        transform.position = new Vector3(bossManager.startPos.x + (13 * mult), bossManager.startPos.y + 2.5f, transform.position.z);
        transform.localScale = new Vector3(9, 9, 1);

        yield return new WaitForSeconds(0.5f);
        GetComponent<Collider2D>().enabled = true;
        bossManager.animator.Play("WallLaugh");
        yield return new WaitForSeconds(0.25f);

        int offset = -2;
        int amount = 8;

        if (bossManager.health.health < bossManager.health.maxHealth / 2)
            amount = 12;

        for(int i = 0; i < amount; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                offset = -offset;
                yield return new WaitForSeconds(0.5f);
            }

            GameObject newProj = Instantiate(projectile, transform.position, transform.rotation);
            newProj.transform.Translate(Vector3.up * offset);
            yield return new WaitForSeconds(0.25f);
        }

        bossManager.animator.Play("DeformWall");
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.55f);

        transform.position = new Vector3(transform.position.x, bossManager.startPos.y - 5, transform.position.z);
        transform.localScale = new Vector3(4, 4, 1);

        yield return new WaitForSeconds(0.3f);
        GetComponent<Collider2D>().enabled = true;

        Attack();
    }

    public IEnumerator MoveAttack()
    {
        yield return new WaitForSeconds(0.25f);

        bossManager.animator.Play("Move");

        int mult = 1;
        moveDir = Vector3.left;

        if (transform.eulerAngles.y == 180)
            mult = -1;

        target = new Vector3(bossManager.startPos.x - (14 * mult), transform.position.y, transform.position.z);
        isMoving = true;

        float speedMult = 1;

        if (bossManager.health.health < bossManager.health.maxHealth / 2)
            speedMult = 1.4f;

        speed = moveSpeed * speedMult;
        currentAttack = "move";

        bossManager.rb.gravityScale = 1;
        GetComponent<Collider2D>().isTrigger = false;
    }

    public IEnumerator DeformWall()
    {
        bossManager.animator.Play("DeformWall");
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.55f);
        StartCoroutine(Death());
    }

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(0.75f);
        currentAttack = "dead";
        transform.eulerAngles = Vector3.zero;
        bossManager.animator.Play("Death");
        transform.position = bossManager.startPos - new Vector3(0, 5, 0);
        transform.localScale = new Vector3(4, 4, 1);
        target = bossManager.startPos;
        moveDir = Vector3.up;
        speed = diveSpeed;
        isMoving = true;
    }
}
