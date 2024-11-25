using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; set; }

    //UI
    public GameObject craftingScreen;
    public GameObject craftingSlot0;
    public GameObject craftingSlot1;
    public GameObject craftingSlot2;
    public GameObject craftingSlot3;
    
    public List<string> inventoryItemList = new List<string>();

    Button craftAxeBtn;
    Button craftPickaxeBtn;
    Button craftHammerBtn;
    Button craftWorkbenchBtn;

    TextMeshProUGUI axeReq1, axeReq2;
    TextMeshProUGUI pickaxeReq1, pickaxeReq2;
    TextMeshProUGUI hammerReq1, hammerReq2;
    TextMeshProUGUI workbenchReq1, workbenchReq2;

    //bool isOpen;

    public ItemBlueprint AxeB = new ItemBlueprint("Axe", 2, "Rocks", 2, "Branch", 2);
    public ItemBlueprint PickaxeB = new ItemBlueprint("Pickaxe", 2, "Rocks", 3, "Branch", 1);
    public ItemBlueprint HammerB = new ItemBlueprint("Hammer", 2, "Rocks", 1, "Branch", 3);
    public ItemBlueprint WorkbenchB = new ItemBlueprint("Workbench", 2, "Wood", 4, "Branch", 4);

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
        //isOpen = false;

        //Getting Craft btn
        craftAxeBtn = craftingScreen.transform.Find("craftAxeBtn").GetComponent<Button>();
        craftPickaxeBtn = craftingScreen.transform.Find("craftPaxeBtn").GetComponent<Button>();
        craftHammerBtn = craftingScreen.transform.Find("craftHammerBtn").GetComponent<Button>();
        craftWorkbenchBtn = craftingScreen.transform.Find("craftWorkbenchBtn").GetComponent<Button>();

        //Axe
        axeReq1 = craftingSlot0.transform.Find("req1").GetComponent<TextMeshProUGUI>();
        axeReq2 = craftingSlot0.transform.Find("req2").GetComponent<TextMeshProUGUI>();
        craftAxeBtn.onClick.AddListener(delegate { CraftAnyItem(AxeB); });
        //Pickaxe
        pickaxeReq1 = craftingSlot1.transform.Find("req1").GetComponent<TextMeshProUGUI>();
        pickaxeReq2 = craftingSlot1.transform.Find("req2").GetComponent<TextMeshProUGUI>();
        craftPickaxeBtn.onClick.AddListener(delegate { CraftAnyItem(PickaxeB); });
        //Hammer
        hammerReq1 = craftingSlot2.transform.Find("req1").GetComponent<TextMeshProUGUI>();
        hammerReq2 = craftingSlot2.transform.Find("req2").GetComponent<TextMeshProUGUI>();
        craftHammerBtn.onClick.AddListener(delegate { CraftAnyItem(HammerB); });
        //Workbench
        workbenchReq1 = craftingSlot3.transform.Find("req1").GetComponent<TextMeshProUGUI>();
        workbenchReq2 = craftingSlot3.transform.Find("req2").GetComponent<TextMeshProUGUI>();
        craftWorkbenchBtn.onClick.AddListener(delegate { CraftAnyItem(WorkbenchB); });
    }

    private void CraftAnyItem(ItemBlueprint itemBlueprint)
    {
        SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.craftingSound);

        //Add item in inventory
        InventorySystem.Instance.AddToInventory(itemBlueprint.itemName);

        //Remove resources from inventory
        if(itemBlueprint.numOfReq == 1)
        {
            InventorySystem.Instance.RemoveItem(itemBlueprint.req1, itemBlueprint.reqAmount1);
        } else if (itemBlueprint.numOfReq == 2)
        {
            InventorySystem.Instance.RemoveItem(itemBlueprint.req1, itemBlueprint.reqAmount1);
            InventorySystem.Instance.RemoveItem(itemBlueprint.req2, itemBlueprint.reqAmount2);
        }

        InventorySystem.Instance.RecalculateList();

        StartCoroutine(Calculate());
    }

    public IEnumerator Calculate()
    {
        yield return 0;
        InventorySystem.Instance.RecalculateList();
        RefreshNeededItems();
    }

    public void RefreshNeededItems()
    {
        int rock_count = 0;
        int branch_count = 0;
        int wood_count = 0;

        inventoryItemList = InventorySystem.Instance.itemList;

        foreach (string itemName in inventoryItemList) 
        {
            switch (itemName)
            {
                case "Rocks":
                    rock_count += 1;
                    break;
                case "Branch":
                    branch_count += 1;
                    break;
                case "Wood":
                    wood_count += 1;
                    break;
            }
        }
        //Axe
        axeReq1.text = "2 Rocks [" + rock_count + "]";
        axeReq2.text = "2 Branch [" + branch_count + "]";

        if (rock_count >= 2 && branch_count >= 2) 
        {
            craftAxeBtn.gameObject.SetActive(true);
        }
        else
        {
            craftAxeBtn.gameObject.SetActive(false);
        }
        //Pickaxe
        pickaxeReq1.text = "3 Rocks [" + rock_count + "]";
        pickaxeReq2.text = "1 Branch [" + branch_count + "]";

        if (rock_count >= 3 && branch_count >= 1)
        {
            craftPickaxeBtn.gameObject.SetActive(true);
        }
        else
        {
            craftPickaxeBtn.gameObject.SetActive(false);
        }
        //Hammer
        hammerReq1.text = "1 Rocks [" + rock_count + "]";
        hammerReq2.text = "3 Branch [" + branch_count + "]";

        if (rock_count >= 1 && branch_count >= 3)
        {
            craftHammerBtn.gameObject.SetActive(true);
        }
        else
        {
            craftHammerBtn.gameObject.SetActive(false);
        }
        //Workbench
        workbenchReq1.text = "4 Woods [" + wood_count + "]";
        workbenchReq2.text = "4 Branch [" + branch_count + "]";

        if (wood_count >= 4 && branch_count >= 4)
        {
            craftWorkbenchBtn.gameObject.SetActive(true);
        }
        else
        {
            craftWorkbenchBtn.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //RefreshNeededItems();
        //if (Input.GetKeyDown(KeyCode.E) && !isOpen)
        //{
        //    Debug.Log("E is pressed");
        //    Cursor.lockState = CursorLockMode.None;
        //    craftingScreen.SetActive(true);
        //    isOpen = true;
        //}
        //else if (Input.GetKeyDown(KeyCode.E) && isOpen)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    craftingScreen.SetActive(false);
        //    isOpen = false;
        //}
    }

}
