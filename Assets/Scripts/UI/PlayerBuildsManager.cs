using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildsManager : MonoBehaviour
{
    public static PlayerBuildsManager Instance { get; set; }

    public GameObject buildings;
    public GameObject buildingsGhost;
    public GameObject placements;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
