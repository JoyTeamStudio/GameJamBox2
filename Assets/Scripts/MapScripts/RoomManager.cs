using Unity.Cinemachine;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject roomCamera;
    public Transform enemies;
    public GameObject camCollider;

    public bool startRoom;
    public bool visitedSinceLastHeal;
    public bool visited;

    public Boss[] bosses;
    public GauntletManager[] gauntlets;

    private void Start()
    {
        camCollider = roomCamera.GetComponent<CinemachineConfiner2D>().BoundingShape2D.gameObject;

        if(startRoom)
            EnterRoom();
    }

    public void LeaveRoom()
    {
        roomCamera.SetActive(false);
        enemies.gameObject.SetActive(false);

        foreach(Boss b in bosses)
            if(!b.hasEnded && b.hasStarted)
                b.ResetBoss();

        foreach (GauntletManager b in gauntlets)
            if (!b.hasFinished && b.hasStarted)
                b.ResetGauntlet();
    }

    public void EnterRoom()
    {
        MainManager.Instance.currentCam = roomCamera;
        MainManager.Instance.currentRoom = this;
        roomCamera.SetActive(true);
        enemies.gameObject.SetActive(true);

        foreach(Transform t in enemies)
        {
            if(!visitedSinceLastHeal)
                t.gameObject.SetActive(true);

            t.gameObject.GetComponent<Enemy>().StartEnemy();
        }

        visitedSinceLastHeal = true;
        visited = true;
    }
}
