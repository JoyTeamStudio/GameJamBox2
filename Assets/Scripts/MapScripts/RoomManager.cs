using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject roomCamera;
    public Transform enemies;

    public bool startRoom;
    public bool visitedSinceLastHeal;

    public Boss[] bosses;

    private void Start()
    {
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
    }
}
