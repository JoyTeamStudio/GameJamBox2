using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

public class BreakableWall : MonoBehaviour
{
    public int hits;
    public GameObject wall;
    public Material flash;

    public void BreakWall()
    {
        hits--;

        StartCoroutine(Flash());

        if(hits <= 0)
        {
            wall.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public IEnumerator Flash()
    {
        Material original = GetComponent<SpriteRenderer>().material;

        GetComponent<SpriteRenderer>().material = flash;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().material = original;
    }
}
