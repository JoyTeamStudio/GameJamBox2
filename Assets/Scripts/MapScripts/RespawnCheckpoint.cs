using UnityEngine;

public class RespawnCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
            collision.gameObject.GetComponent<PlayerHealth>().respawnPoint = transform.position;
    }
}
