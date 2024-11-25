using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotSystem : MonoBehaviour
{
    public static QuickSlotSystem Instance { get; set; }

    // UI 
    public GameObject quickSlotsPanel;
    public GameObject numbersHolder;

    public List<GameObject> quickSlotsList = new List<GameObject>();

    public int selectedNumber = -1;
    public GameObject selectedItem;

    public GameObject toolHolder;
    public GameObject selectedItemModel;

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

    private void Start()
    {
        PopulateSlotList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SelectQuickSlot(1);
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            SelectQuickSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectQuickSlot(3);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectQuickSlot(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectQuickSlot(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectQuickSlot(6);
        }
    }

    void SelectQuickSlot(int num)
    {
        if (checkIfSlotIsFull(num) == true)
        {
            if (selectedNumber != num)
            {
                selectedNumber = num;

                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                }

                selectedItem = getSelectedItem(num);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                SetEquippedModel(selectedItem);
                SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.itemInteractSound);

                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("qslot_number_text").GetComponent<TextMeshProUGUI>().color = Color.black;
                }

                TextMeshProUGUI toBechanged = numbersHolder.transform.Find("qslot_number" + num).transform.Find("qslot_number_text").GetComponent<TextMeshProUGUI>();
                toBechanged.color = Color.white;
            }
            else
            {
                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;

                    ConstructionManager.Instance.inConstructionMode = false;
                    ConstructionManager.Instance.CancelBuilding();
                }

                if (selectedItemModel != null)
                {
                    DestroyImmediate(selectedItemModel.gameObject);
                    selectedItemModel = null;
                }

                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("qslot_number_text").GetComponent<TextMeshProUGUI>().color = Color.black;
                }

                selectedNumber = -1; //null
            }
        }
    }

    GameObject getSelectedItem(int slotNumber)
    {
        return quickSlotsList[slotNumber-1].transform.GetChild(0).gameObject;
    }

    private void SetEquippedModel(GameObject selectedItem)
    {
        if (selectedItemModel != null)
        {
            DestroyImmediate(selectedItemModel.gameObject);
            selectedItemModel = null;
        }

        string selectedItemName = selectedItem.name.Replace("(Clone)","");
        selectedItemModel = Instantiate(Resources.Load<GameObject>("Items_model/" + selectedItemName + "_Model"),
            new Vector3(0.75f, 0.8f, 1.1f), Quaternion.Euler(-12.6f, -94f, 5.6f));
        selectedItemModel.transform.SetParent(toolHolder.transform, false);
    }

    bool checkIfSlotIsFull(int slotNum)
    {
        if (quickSlotsList[slotNum-1].transform.childCount > 0)
        { 
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        // Find next free slot
        GameObject availableSlot = FindNextEmptySlot();
        // Set transform of our object
        itemToEquip.transform.SetParent(availableSlot.transform, false);

        InventorySystem.Instance.RecalculateList();
    }

    public GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
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

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
