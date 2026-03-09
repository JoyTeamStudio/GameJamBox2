using UnityEngine;

public class PlayerHitCollision : MonoBehaviour
{
    private PlayerHealth health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = GetComponentInParent<PlayerHealth>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            if(collision.gameObject.GetComponent<Boss>() != null || collision.gameObject.GetComponent<Enemy>().hitPlayer)
                    health.TakeDamage();
        

        if (collision.gameObject.CompareTag("Money"))
            collision.gameObject.GetComponent<MoneyCoin>().GiveMoney();
    }
}
