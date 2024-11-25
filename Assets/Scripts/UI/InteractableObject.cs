using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName;
    public bool isDestroyable;

    public string GetItemName()
    {
        return ItemName;
    }

    public bool IsDestroyable()
    {
        return isDestroyable;
    }    
}
