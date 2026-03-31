using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

public class BreakableWall : MonoBehaviour
{
    public string wallName;
    public int hits;
    public GameObject wall;
    public Material flash;

    private void Start()
    {
        for (int i = 0; i < MainManager.Instance.walls.Length; i++)
        {
            if (wallName.ToLower() == MainManager.Instance.walls[i].wallName.ToLower())
            {
                wall.SetActive(!MainManager.Instance.walls[i].broken);
                gameObject.SetActive(!MainManager.Instance.walls[i].broken);
                break;
            }
        }
    }

    public void BreakWall()
    {
        hits--;
        GetComponent<AudioSource>().Play();

        StartCoroutine(Flash());

        if(hits <= 0)
        {
            wall.SetActive(false);
            gameObject.SetActive(false);

            for (int i = 0; i < MainManager.Instance.walls.Length; i++)
            {
                if (wallName.ToLower() == MainManager.Instance.walls[i].wallName.ToLower())
                {
                    MainManager.Instance.walls[i].broken = true;
                    break;
                }
            }

            MainManager.Instance.SavePlayer();
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
