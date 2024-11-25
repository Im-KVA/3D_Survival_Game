using Den.Tools;
using MapMagic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMaker : MonoBehaviour
{
    public static WorldMaker Instance { get; set; }
    public MapMagicObject mapMagic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void WorldBuild(int seed)
    {
        if (mapMagic != null)
        {
            mapMagic.graph.random = new Noise(seed, permutationCount: 32768);
        }

        mapMagic.Refresh();
    }
    
    public int SeedMaker()
    {
        return Random.Range(1, 99999);
    }
}

[System.Serializable]
public class WorldData
{
    public int seed;

    public WorldData(int temp_seed)
    {
        seed = temp_seed;
    }
}

