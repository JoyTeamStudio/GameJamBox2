using System.Collections;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    public float fadeSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        while(spriteRenderer.color.a > 0)
        {
            spriteRenderer.color -= new Color32(0, 0, 0, (byte)(fadeSpeed * Time.deltaTime));
            yield return null;
        }

        Destroy(gameObject);
    }
}
