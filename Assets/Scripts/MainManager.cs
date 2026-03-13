using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public GameObject currentCam;
    public RoomManager currentRoom;

    public List<Boss> defeatedBosses;

    public bool shopping;

    public bool hasDash;

    public int money;
    public int hpPieces;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddBoss(Boss boss)
    {
        if(!defeatedBosses.Contains(boss))
            defeatedBosses.Add(boss);
    }

    public void FocusCamera(Transform focus)
    {
        currentCam.GetComponent<CinemachineCamera>().Follow = focus;
    }
}
