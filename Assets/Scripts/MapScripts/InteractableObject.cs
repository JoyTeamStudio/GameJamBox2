using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string objectName;
    public string itemName;
    public Sprite icon;

    public bool playerIn;
    public GameObject interactText;

    private GameObject player;
    private GameManager gameManager;

    private void Start()
    {
        interactText.SetActive(false);
        player = GameObject.FindWithTag("Player");
        gameManager = FindAnyObjectByType<GameManager>();

        for (int i = 0; i < MainManager.Instance.items.Length; i++)
        {
            if (itemName.ToLower() == MainManager.Instance.items[i].itemName.ToLower())
            {
                gameObject.SetActive(!MainManager.Instance.items[i].claimed);
                break;
            }
        }
    }

    private void Update()
    {
        if (playerIn && Input.GetButtonDown("Vertical"))
        {
            gameManager.ObtainObject(objectName, GetComponent<SpriteRenderer>().sprite);
            gameObject.SetActive(false);

            for (int i = 0; i < MainManager.Instance.items.Length; i++)
            {
                if (itemName.ToLower() == MainManager.Instance.items[i].itemName.ToLower())
                {
                    MainManager.Instance.items[i].claimed = true;
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIn = true;
            interactText.SetActive(playerIn);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIn = false;
            interactText.SetActive(playerIn);
        }
    }
}
