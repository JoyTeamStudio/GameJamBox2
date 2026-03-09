using UnityEngine;

public class GateLever : MonoBehaviour
{
    public Animator gate;
    public bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Projectile") && !triggered)
        {
            Destroy(collision.gameObject);
            triggered = true;
            gate.Play("Open");
            gate.gameObject.GetComponentInChildren<Collider2D>().enabled = false;
        }
    }
}
