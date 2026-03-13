using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public Boss boss;
    public GauntletManager gauntlet;

    public string type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(type == "boss" && !boss.hasStarted)
            {
                boss.StartBoss();
                return;
            }

            if (type == "gauntlet" && !gauntlet.hasStarted)
            {
                gauntlet.StartGauntlet();
            }
        }
    }
}
