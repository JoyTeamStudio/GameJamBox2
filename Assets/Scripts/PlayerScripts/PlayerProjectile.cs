using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed;
    public float maxDistance;
    private float counter;
    public int damage;

    public PlayerAttack attack;

    public bool enemyProjectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Time.deltaTime * speed;
        transform.Translate(dist * Vector3.right);

        counter += dist;

        if(counter > maxDistance) 
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy") && !enemyProjectile)
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            attack.IncreaseHealCount();
            Destroy(gameObject);
        }

        if(collision.gameObject.CompareTag("Player") && enemyProjectile)
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage();
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
