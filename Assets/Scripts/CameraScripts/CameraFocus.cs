using Unity.Cinemachine;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    private CinemachineCamera cam;
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        cam = GetComponentInParent<RoomManager>().roomCamera.GetComponent<CinemachineCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            cam.Follow = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            cam.Follow = player;
        }
    }
}
