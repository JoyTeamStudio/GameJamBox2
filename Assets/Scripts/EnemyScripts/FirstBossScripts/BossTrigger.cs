using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public Boss boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !boss.hasStarted)
        {
            boss.StartBoss();
            foreach(Door d in boss.doors)
                d.TriggerDoor();
        }
    }
}
