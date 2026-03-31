using UnityEngine;
using UnityEngine.SceneManagement;

public class RollCredits : MonoBehaviour
{
    public float speed;
    public float bound;
    private bool endedCredits = false;

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition.y < bound)
        {
            transform.Translate(speed * Time.deltaTime * Vector3.up);
        }
        else if (!endedCredits)
        {
            endedCredits = true;
            Invoke(nameof(BackToMenu), 5);
        }
    }

    public void BackToMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}
