using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageBoxData
{
    public string boxName;

    public float[] position;
    public float[] rotation;

    public List<string> items;

    public StorageBoxData(string boxName, float[] position, float[] rotation, List<string> items)
    {
        this.boxName = boxName;
        this.position = position;
        this.rotation = rotation;
        this.items = items;
    }
}
