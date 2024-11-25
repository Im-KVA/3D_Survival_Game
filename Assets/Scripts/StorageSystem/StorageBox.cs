using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox : MonoBehaviour
{
    public bool playerInRange;

    public List<string> items;
    
    public enum BoxType
    {
        smallBox, largeBox
    }

    public BoxType thisBoxType;

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(PlayerStateSystem.Instance.player.transform.position, this.transform.position);

        if (distance < 10f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }
}
