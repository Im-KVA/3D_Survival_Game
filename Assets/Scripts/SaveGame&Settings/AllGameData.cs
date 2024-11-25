using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class AllGameData
{
    public PlayerData playerData;
    public WorldData worldData;
    public List<BuildingsData> savedBuildingsList;
    public List<StorageBoxData> storageBoxDataList;
}
