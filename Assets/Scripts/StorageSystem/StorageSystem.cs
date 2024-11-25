using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageSystem : MonoBehaviour
{
    public static StorageSystem Instance { get; set; }

    [SerializeField] GameObject storageBoxSmallUI;
    [SerializeField] StorageBox selectedStorage;
    public bool storageUIOpen;

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

    public void OpenBox(StorageBox storage)
    {
        SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.itemSound);

        SetSelectedStorage(storage);

        PopulateStorage(GetRelevantUI(selectedStorage));

        GetRelevantUI(selectedStorage).SetActive(true);
        storageUIOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        InventorySystem.Instance.inventoryScreenUI.SetActive(true);
        InventorySystem.Instance.isOpen = true;

        PlayerStateSystem.Instance.playerInforUI.SetActive(false);

        ActionManager.Instance.DisableSelection();
        ActionManager.Instance.GetComponent<ActionManager>().enabled = false;
    }

    private void PopulateStorage(GameObject storageUI)
    {
        // Get all slots of the ui
        List<GameObject> uiSlots = new List<GameObject>();

        foreach (Transform child in storageUI.transform)
        {
            uiSlots.Add(child.gameObject);
        }

        // Now, instantiate the prefab and set it as a child of each GameObject
        foreach (string name in selectedStorage.items)
        {
            foreach (GameObject slot in uiSlots)
            {
                if (slot.transform.childCount < 1 && slot.CompareTag("Slot"))
                {
                    var itemToAdd = Instantiate(Resources.Load<GameObject>("Items/" + name), slot.transform.position, slot.transform.rotation);    

                    itemToAdd.name = name;

                    itemToAdd.transform.SetParent(slot.transform);
                    break;
                }
            }
        }
    }

    public void CloseBox()
    {
        SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.itemSound);

        RecalculateStorage(GetRelevantUI(selectedStorage));

        GetRelevantUI(selectedStorage).SetActive(false);
        storageUIOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InventorySystem.Instance.inventoryScreenUI.SetActive(false);
        InventorySystem.Instance.isOpen = false;

        PlayerStateSystem.Instance.playerInforUI.SetActive(true);

        ActionManager.Instance.EnableSelection();
        ActionManager.Instance.GetComponent<ActionManager>().enabled = true;
    }

    private void RecalculateStorage(GameObject storageUI)
    {
        List<GameObject> uiSlots = new List<GameObject>();

        foreach (Transform child in storageUI.transform) 
        {
            uiSlots.Add(child.gameObject);
        }

        selectedStorage.items.Clear();  

        List<GameObject> toBeDeleted = new List<GameObject>();

        foreach (GameObject slot in uiSlots)
        {
            if (slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name;
                string temp = "(Clone)";
                string result = name.Replace(temp, "");

                selectedStorage.items.Add(result);
                toBeDeleted.Add(slot.transform.GetChild(0).gameObject);
            }
        }

        foreach (GameObject objects in toBeDeleted)
        {
            Destroy(objects);
        }
    }

    public void SetSelectedStorage(StorageBox storage)
    {
        selectedStorage = storage;
    }

    private GameObject GetRelevantUI(StorageBox storage)
    {
        // Create a switch for other types
        return storageBoxSmallUI;
    }
}
