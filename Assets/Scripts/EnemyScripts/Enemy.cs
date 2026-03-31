using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string defaultAnimation;
    public Animator animator;
    public GameObject player;
    public Vector3 target;

    public GameObject projectile;

    public float chaseDistance;
    public float attackDistance;
    public float idleWalkDistance;
    public float distToPlayer;

    public float walkSpeed;
    public float attackDelay;

    public Vector3 initialPos;

    public bool attacking;

    public enum State { Idle, Chase, Attack }
    public State state;

    public bool chase;
    public bool attack;
    public bool hitPlayer;
    public bool gauntlet;


    public EnemyCollide wallCollider;
    public EnemyCollide groundCollider;
    public bool collideWall;
    public bool collideGround;
    public bool checkingCollide;

    [System.Serializable]
    public class Coins
    {
        public GameObject coinType;
        public int amount;
    }

    public Coins[] coins;

    private void Awake()
    {
        initialPos = transform.position;
    }

    public void StartEnemy()
    {
        if(GetComponent<Animator>() != null)
        {
            animator = GetComponent<Animator>();
            animator.Play(defaultAnimation);
        }

        checkingCollide = true;
        transform.position = initialPos;
        GetComponent<PlayerHealth>().health = GetComponent<PlayerHealth>().maxHealth;
        player = FindAnyObjectByType<PlayerMovement>(FindObjectsInactive.Include).gameObject;
        distToPlayer = Vector3.Distance(player.transform.position, transform.position);
        target = new Vector3(initialPos.x + idleWalkDistance, transform.position.y, transform.position.z);
        state = State.Idle;
        attacking = false;
    }

    private void Update()
    {
        distToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if(wallCollider != null)
            collideWall = wallCollider.collided;
        if(groundCollider != null)
            collideGround = groundCollider.collided;
    }

    public void GiveMoney()
    {
        if (gauntlet)
            return;

        for(int i = 0; i < coins.Length; i++)
        {
            for (int j = 0; j < coins[i].amount; j++)
            {
                GameObject newCoin = Instantiate(coins[i].coinType, transform.position, transform.rotation);
                newCoin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-2.0f, 2.0f), 2), ForceMode2D.Impulse);
            }
        }
    }

    public void ResetColliders()
    {
        StartCoroutine(ColliderCooldown());
    }

    private IEnumerator ColliderCooldown()
    {
        checkingCollide = false;
        yield return new WaitForSeconds(0.2f);
        checkingCollide = true;
    }


}
