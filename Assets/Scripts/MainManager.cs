using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Enemy;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public GameObject currentCam;
    public RoomManager currentRoom;

    public List<Boss> defeatedBosses;

    public int maxSlots;

    public bool shopping;

    public bool hasStarted;
    public bool learnedHealing;
    public bool learnedInteract;

    public bool hasDash;
    public bool hasDoubleJump;
    public bool hasMap;

    public bool obtainedFinalItem;

    public int money;
    public int hpPieces;
    public int slots;
    public int minerals;
    public int keys;
    public int weaponLevel;

    public int[] boughtShopItems;

    public string currentStation;

    [System.Serializable]
    public class NPCDialogueData
    {
        public string npcName;
        public int dialogueIndex;
    }
    public NPCDialogueData[] npcData;

    [System.Serializable]
    public class GauntletBoss
    {
        public string gaunletName;
        public bool finished;
    }
    public GauntletBoss[] gauntletBossData;

    [System.Serializable]
    public class Room
    {
        public string roomName;
        public bool visited;
    }
    public Room[] rooms;

    [System.Serializable]
    public class Item
    {
        public string itemName;
        public bool claimed;
    }
    public Item[] items;

    [System.Serializable]
    public class Station
    {
        public string stationName;
        public bool used;
    }
    public Station[] stations;

    [System.Serializable]
    public class BreakableWallData
    {
        public string wallName;
        public bool broken;
    }
    public BreakableWallData[] walls;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPlayer();
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

    public void SavePlayer()
    {
        PlayerSaveData playerData = new PlayerSaveData();
        playerData.hasStarted = hasStarted;
        playerData.learnedHealing = learnedHealing;
        playerData.learnedInteract = learnedInteract;
        playerData.hasMap = hasMap;
        playerData.hasDash = hasDash;
        playerData.hasDoubleJump = hasDoubleJump;
        playerData.hasFinalItem = obtainedFinalItem;
        playerData.money = money;
        playerData.hpPieces = hpPieces;
        playerData.slots = slots;
        playerData.keys = keys;
        playerData.minerals = minerals;
        playerData.weaponLevel = weaponLevel;
        playerData.shopBoughtItems = boughtShopItems;
        playerData.currentStation = currentStation;

        NPCDialogueSaveData[] npcData = new NPCDialogueSaveData[this.npcData.Length];

        for (int i = 0; i < npcData.Length; i++)
        {
            NPCDialogueSaveData currentNPC = new NPCDialogueSaveData();
            currentNPC.npcName = this.npcData[i].npcName;
            currentNPC.dialogueIndex = this.npcData[i].dialogueIndex;

            npcData[i] = currentNPC;
        }

        GauntletBossSaveData[] gauntletBosses = new GauntletBossSaveData[gauntletBossData.Length];

        for (int i = 0; i < gauntletBosses.Length; i++)
        {
            GauntletBossSaveData currentGauntlet = new GauntletBossSaveData();
            currentGauntlet.gauntletName = gauntletBossData[i].gaunletName;
            currentGauntlet.finished = gauntletBossData[i].finished;

            gauntletBosses[i] = currentGauntlet;
        }

        RoomSaveData[] roomData = new RoomSaveData[rooms.Length];

        for (int i = 0; i < roomData.Length; i++)
        {
            RoomSaveData currentRoom = new RoomSaveData();
            currentRoom.roomName = rooms[i].roomName;
            currentRoom.visited = rooms[i].visited;

            roomData[i] = currentRoom;
        }

        ItemSaveData[] itemSaveData = new ItemSaveData[items.Length];

        for (int i = 0; i < itemSaveData.Length; i++)
        {
            ItemSaveData currentItem = new ItemSaveData();
            currentItem.itemName = items[i].itemName;
            currentItem.claimed = items[i].claimed;

            itemSaveData[i] = currentItem;
        }

        StationSaveData[] stationSaveData = new StationSaveData[stations.Length];

        for (int i = 0; i < stationSaveData.Length; i++)
        {
            StationSaveData currentStation = new StationSaveData();
            currentStation.stationName = stations[i].stationName;
            currentStation.used = stations[i].used;

            stationSaveData[i] = currentStation;
        }

        BreakableWallSaveData[] breakableWallSaveData = new BreakableWallSaveData[walls.Length];

        for (int i = 0; i < breakableWallSaveData.Length; i++)
        {
            BreakableWallSaveData currentWall = new BreakableWallSaveData();
            currentWall.wallName = walls[i].wallName;
            currentWall.broken = walls[i].broken;

            breakableWallSaveData[i] = currentWall;
        }

        SaveSystem.Save(new SaveData(playerData, npcData, gauntletBosses, roomData, itemSaveData, stationSaveData, breakableWallSaveData));
    }

    public void LoadPlayer()
    {
        SaveData data = SaveSystem.Load();

        if (data != null)
        {
            hasStarted = data.playerData.hasStarted;
            learnedHealing = data.playerData.learnedHealing;
            learnedInteract = data.playerData.learnedInteract;
            hasMap = data.playerData.hasMap;
            hasDash = data.playerData.hasDash;
            hasDoubleJump = data.playerData.hasDoubleJump;
            obtainedFinalItem = data.playerData.hasFinalItem;
            money = data.playerData.money;
            hpPieces = data.playerData.hpPieces;
            slots = data.playerData.slots;
            keys = data.playerData.keys;
            minerals = data.playerData.minerals;
            weaponLevel = data.playerData.weaponLevel;
            boughtShopItems = data.playerData.shopBoughtItems;
            currentStation = data.playerData.currentStation;

            NPCDialogueSaveData[] npcData = data.npcDialogueSaveData;

            for (int i = 0; i < npcData.Length; i++)
            {
                this.npcData[i].dialogueIndex = npcData[i].dialogueIndex;
            }

            GauntletBossSaveData[] gauntletBosses = data.gauntletBossSaveData;

            for (int i = 0; i < gauntletBosses.Length; i++)
            {
                gauntletBossData[i].finished = gauntletBosses[i].finished;
            }

            RoomSaveData[] roomSaveDatas = data.roomSaveData;

            for (int i = 0; i < roomSaveDatas.Length; i++)
            {
                rooms[i].visited = roomSaveDatas[i].visited;
            }

            ItemSaveData[] itemSaveDatas = data.itemSaveData;

            for (int i = 0; i < itemSaveDatas.Length; i++)
            {
                items[i].claimed = itemSaveDatas[i].claimed;
            }

            StationSaveData[] stationSaveDatas = data.stationSaveData;

            for (int i = 0; i < stationSaveDatas.Length; i++)
            {
                stations[i].used = stationSaveDatas[i].used;
            }

            BreakableWallSaveData[] breakableWallDatas = data.breakableWallSaveData;

            for (int i = 0; i < breakableWallDatas.Length; i++)
            {
                walls[i].broken = breakableWallDatas[i].broken;
            }
        }

        SavePlayer();
    }
}
