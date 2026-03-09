using UnityEngine;

public class MoneyCoin : MonoBehaviour
{
    public int value;

    public void GiveMoney()
    {
        FindAnyObjectByType<GameManager>().GetMoney(value);
        Destroy(gameObject);
    }
}
