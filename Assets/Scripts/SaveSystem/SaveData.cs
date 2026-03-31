using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerSaveData playerData;
    public NPCDialogueSaveData[] npcDialogueSaveData;
    public GauntletBossSaveData[] gauntletBossSaveData;
    public RoomSaveData[] roomSaveData;
    public ItemSaveData[] itemSaveData;
    public StationSaveData[] stationSaveData;
    public BreakableWallSaveData[] breakableWallSaveData;

    public SaveData(PlayerSaveData playerData, NPCDialogueSaveData[] npcDialogueSaveData, 
        GauntletBossSaveData[] gauntletBossSaveData, RoomSaveData[] roomSaveData, ItemSaveData[] itemData, 
        StationSaveData[] stationSaveData, BreakableWallSaveData[] breakableWallSaveData)
    {
        this.playerData = playerData;
        this.npcDialogueSaveData = npcDialogueSaveData;
        this.gauntletBossSaveData = gauntletBossSaveData;
        this.roomSaveData = roomSaveData;
        this.itemSaveData = itemData;
        this.stationSaveData = stationSaveData;
        this.breakableWallSaveData = breakableWallSaveData;
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public bool hasStarted;
    public bool learnedHealing;
    public bool learnedInteract;
    public bool hasMap;
    public bool hasDash;
    public bool hasDoubleJump;
    public bool hasFinalItem;

    public int money;
    public int hpPieces;
    public int slots;
    public int keys;
    public int minerals;
    public int weaponLevel;

    public int[] shopBoughtItems;

    public string currentStation;
}

[System.Serializable]
public class NPCDialogueSaveData
{
    public string npcName;
    public int dialogueIndex;
}

[System.Serializable]
public class GauntletBossSaveData
{
    public string gauntletName;
    public bool finished;
}

[System.Serializable]
public class RoomSaveData
{
    public string roomName;
    public bool visited;
}

[System.Serializable]
public class ItemSaveData
{
    public string itemName;
    public bool claimed;
}

[System.Serializable]
public class StationSaveData
{
    public string stationName;
    public bool used;
}

[System.Serializable]
public class BreakableWallSaveData
{
    public string wallName;
    public bool broken;
}