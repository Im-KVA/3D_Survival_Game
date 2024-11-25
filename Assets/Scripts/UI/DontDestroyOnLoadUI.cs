using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadUI : MonoBehaviour
{
    public static DontDestroyOnLoadUI Instance { get; set; }

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
