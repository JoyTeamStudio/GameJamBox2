using UnityEngine;

public class DealDamageToPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
        {
            if (collision.gameObject.CompareTag("Player"))
                collision.GetComponent<PlayerHealth>().TakeDamage();
            
            Destroy(gameObject);
        }
    }
}
