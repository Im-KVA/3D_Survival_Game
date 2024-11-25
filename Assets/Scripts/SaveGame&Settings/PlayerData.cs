using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public float[] playerStats; //[0] - health; [1] - energy
    public float[] playerPositionAndRotation;

    public string[] inventoryItems;
    public string[] quickSlotItems;

    public PlayerData(float[] temp_playerStats, 
        float[] temp_playerPositionAndRotation, 
        string[] temp_inventoryItem, 
        string[] temp_quickSlotItems)
    {
        playerStats = temp_playerStats;
        playerPositionAndRotation = temp_playerPositionAndRotation;
        inventoryItems = temp_inventoryItem;
        quickSlotItems = temp_quickSlotItems;
    }
}
