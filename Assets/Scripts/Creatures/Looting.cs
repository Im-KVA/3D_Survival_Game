using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looting : MonoBehaviour
{
    public List<LootingBank> lootingBank;
    public List<LootingFinal> finalLoot;

    public bool lootingCalculated;


}
[System.Serializable]
public class LootingBank
{
    public GameObject item;
    public int min;
    public int max;
}

[System.Serializable]
public class LootingFinal
{
    public GameObject item;
    public int amount;
}