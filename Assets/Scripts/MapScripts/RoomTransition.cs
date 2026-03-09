using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomTransition : MonoBehaviour
{
    public Transform room;
    public Image transitionFade;
    private GameObject mainCamera;
    private RoomManager roomManager;

    public enum Direction { Right, Left, Up, Down }
    public Direction direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        roomManager = GetComponentInParent<RoomManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement movement = collision.gameObject.GetComponent<PlayerMovement>();

            if(!movement.isTransitioning)
            {
                StartCoroutine(Transition(collision.gameObject, movement));
            }
        }
    }

    private IEnumerator Transition(GameObject player, PlayerMovement movement)
    {
        movement.StartTransition(direction);
        player.SetActive(false);

        for(int i = 0; i < 255; i += 15)
        {
            transitionFade.color = new Color32(0, 0, 0, (byte)i);
            yield return new WaitForSeconds(0.015f);
        }
        transitionFade.color = new Color32(0, 0, 0, 255);

        roomManager.LeaveRoom();
        room.parent.GetComponent<RoomManager>().EnterRoom();
        player.transform.position = room.position;

        yield return new WaitForSeconds(0.5f);

        for (int i = 255; i > 0; i -= 15)
        {
            transitionFade.color = new Color32(0, 0, 0, (byte)i);
            yield return new WaitForSeconds(0.015f);
        }
        transitionFade.color = new Color32(0, 0, 0, 0);

        player.SetActive(true);
        movement.Transition();
    }
}
