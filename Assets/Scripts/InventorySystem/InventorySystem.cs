using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    public GameObject itemInfoUI;
    public GameObject itemAdd;
    public GameObject slotUsed;

    public bool isOpen;
    //public bool isFull;

    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();

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

    void Start()
    {
        isOpen = false;

        PopulateSlotList();

        //Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isOpen && !StorageSystem.Instance.storageUIOpen && !MainMenuManager.Instance.isOpen)
        {
            SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.itemSound);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            CraftingSystem.Instance.craftingScreen.SetActive(true);
            inventoryScreenUI.SetActive(true);

            PlayerStateSystem.Instance.playerInforUI.SetActive(false);

            ActionManager.Instance.DisableSelection();
            ActionManager.Instance.GetComponent<ActionManager>().enabled = false;
            
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.E) && isOpen)
        {
            SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.itemSound);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CraftingSystem.Instance.craftingScreen.SetActive(false);
            inventoryScreenUI.SetActive(false);

            PlayerStateSystem.Instance.playerInforUI.SetActive(true);

            if (StorageSystem.Instance.storageUIOpen)
            {
                StorageSystem.Instance.CloseBox();
            }

            ActionManager.Instance.EnableSelection();
            ActionManager.Instance.GetComponent<ActionManager>().enabled = true;

            isOpen = false;
        }
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    public void AddToInventory(string itemName)
    {
        slotUsed = FindNextSlot();
        itemAdd = Instantiate(Resources.Load<GameObject>("Items/" + itemName), slotUsed.transform.position, slotUsed.transform.rotation);
        itemAdd.transform.SetParent(slotUsed.transform);

        itemList.Add(itemName);

        Sprite sprite = itemAdd.GetComponent<Image>().sprite;
        PopupNotificationSystem.Instance.ShowNotification(itemName, sprite);

        RecalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }

    public void RemoveItem(string itemToRemove, int amountToRemove)
    {
        int count = amountToRemove;
        for (var i = slotList.Count - 1; i >= 0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                if (slotList[i].transform.GetChild(0).name == itemToRemove + "(Clone)" && count != 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    count -= 1;
                }
                else if (slotList[i].transform.GetChild(0).name == itemToRemove && count != 0)
                {
                    Destroy(slotList[i].transform.GetChild(0).gameObject);
                    count -= 1;
                }
            }
        }
        RecalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }

    public void RecalculateList()
    {
        itemList.Clear();

        foreach(GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name;
                string other = "(Clone)";
                string result = name.Replace(other, "");

                itemList.Add(result);
            }
        }
    }

    private GameObject FindNextSlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckIfFull()
    {
        int counter = 0;
        foreach (GameObject slot in slotList)
        {
            if(slot.transform.childCount > 0)
            {
                counter += 1;

            }
        }
        if (counter == 12)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}