using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public int hits;
    public GameObject wall;

    public void BreakWall()
    {
        hits--;

        if(hits <= 0)
        {
            wall.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
