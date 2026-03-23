using System.Collections;
using UnityEngine;

public class FirstBoss : MonoBehaviour
{
    private Boss bossManager;

    public float jumpXForce;
    public float jumpYForce;

    public GameObject throwProjectile;
    public GameObject shootProjectile;

    public float throwTime;
    public float shootTime;
    public int startAmount;
    public float jumpTime;

    private void Start()
    {
        bossManager = GetComponent<Boss>();
    }

    public void Attack()
    {
        int attack = Random.Range(0, 2);

        switch(attack)
        {
            case 0:
                StartCoroutine(ThrowAttack());
                break;
            case 1:
                StartCoroutine(ShootAttack());
                break;
        }
    }

    private IEnumerator Jump()
    {
        bossManager.animator.Play("Jump");
        yield return new WaitForSeconds(jumpTime);

        bossManager.rb.AddForce(new Vector2(jumpXForce * GetSideMult(), jumpYForce), ForceMode2D.Impulse);
        transform.eulerAngles += new Vector3(0, 180, 0);

        yield return new WaitForSeconds(jumpTime * 3);

        Attack();
    }

    private IEnumerator ThrowAttack()
    {
        yield return new WaitForSeconds(jumpTime);
        int amount = startAmount;

        if (bossManager.health.health <= bossManager.health.maxHealth / 2)
            amount++;

        if (bossManager.health.health <= bossManager.health.maxHealth / 5)
            amount++;

        for (int i = 0; i < amount; i++)
        {
            bossManager.animator.Play("Throw");
            yield return new WaitForSeconds(throwTime);
            GameObject newProj = Instantiate(throwProjectile, transform.position, transform.rotation);
            newProj.GetComponent<Rigidbody2D>().AddForce(new Vector2(Vector3.Distance(transform.position, bossManager.player.transform.position) * 0.8f * GetSideMult(), 25), ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(jumpTime);

        StartCoroutine(Jump());
    }

    private IEnumerator ShootAttack()
    {
        yield return new WaitForSeconds(jumpTime);
        int amount = startAmount;

        if (bossManager.health.health <= bossManager.health.maxHealth / 2)
            amount++;

        if (bossManager.health.health <= bossManager.health.maxHealth / 5)
            amount++;

        for (int i = 0; i < amount; i++)
        {
            bossManager.animator.Play("Shoot");
            yield return new WaitForSeconds(shootTime);
            GameObject newProj = Instantiate(shootProjectile, transform.position, transform.rotation);
            newProj.transform.Translate(new Vector3(0, -0.75f, 0));
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(jumpTime);

        StartCoroutine(Jump());
    }

    private int GetSideMult()
    {
        int mult = 1;

        Debug.Log(transform.eulerAngles.y);

        if ((int)transform.eulerAngles.y != 0)
            mult = -1;

        return mult;
    }
}
