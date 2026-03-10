using UnityEngine;

public class EnemyCollide : MonoBehaviour
{
    public bool collided;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
            collided = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            collided = false;
    }
}
