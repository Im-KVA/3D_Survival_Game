using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    //Is this item trashable
    public bool isTrashable;

    //Item Info UI
    private GameObject itemInfoUI;

    private TextMeshProUGUI itemInfoUI_itemName;
    private TextMeshProUGUI itemInfoUI_itemDescription;
    private TextMeshProUGUI itemInfoUI_itemFunction;

    public string thisName, thisDescription, thisFunctionality;

    //Consumption
    private GameObject itemPendingConsumption;
    public bool isConsumable;

    public float healthEffect;
    public float energyEffect;

    //Equipping
    public bool isEquippable;
    private GameObject itemPendingEquipping;
    public bool isInsideQuickSlot;
    public bool isSelected;

    //Build & Placement
    public bool isUseable;

    private void Start()
    {
        itemInfoUI = InventorySystem.Instance.itemInfoUI;
        itemInfoUI_itemName = itemInfoUI.transform.Find("itemName").GetComponent<TextMeshProUGUI>();
        itemInfoUI_itemDescription = itemInfoUI.transform.Find("itemDescription").GetComponent<TextMeshProUGUI>();
        itemInfoUI_itemFunction = itemInfoUI.transform.Find("itemFunction").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (isSelected)
        {
            gameObject.GetComponent<DragDrop>().enabled = false;
        }
        else
        {
            gameObject.GetComponent <DragDrop>().enabled = true;
        }
    }

    // Triggered when the mouse enters into the area of the item that has this script.
    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text = thisName;
        itemInfoUI_itemDescription.text = thisDescription;
        itemInfoUI_itemFunction.text = thisFunctionality;
    }

    // Triggered when the mouse exits the area of the item that has this script.
    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }

    // Triggered when the mouse is clicked over the item that has this script.
    public void OnPointerDown(PointerEventData eventData)
    {
        //Right Mouse Button Click on
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable)
            {
                SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.eatItemSound);   

                itemPendingConsumption = gameObject;
                consumingFunction(healthEffect, energyEffect);
            }

            if (isEquippable && isInsideQuickSlot == false && QuickSlotSystem.Instance.CheckIfFull() == false)
            {
                SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.itemInteractSound);

                QuickSlotSystem.Instance.AddToQuickSlots(gameObject);
                isInsideQuickSlot = true;
            }

            if (isUseable)
            {
                SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.itemInteractSound);

                PlacementManager.Instance.inventoryItemToDestroy = gameObject;
                gameObject.SetActive(false);
                UseItem();
            }
        }  
    }

    // Triggered when the mouse button is released over the item that has this script.
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable && itemPendingConsumption == gameObject)
            {
                DestroyImmediate(gameObject);
                InventorySystem.Instance.RecalculateList();
                CraftingSystem.Instance.RefreshNeededItems();
            }
        }
    }

    private void UseItem()
    {
        InventorySystem.Instance.isOpen = false;
        InventorySystem.Instance.inventoryScreenUI.SetActive(false);

        CraftingSystem.Instance.craftingScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ActionManager.Instance.EnableSelection();
        ActionManager.Instance.enabled = true;

        switch (gameObject.name)
        {
            case "Box(Clone)":
                PlacementManager.Instance.ActivatePlacementMode("Box");
                break;
            case "Box": //Testing
                PlacementManager.Instance.ActivatePlacementMode("Box");
                break;
            default: 
                break;
        }
    }

    private void consumingFunction(float healthEffect, float energyEffect)
    {
        itemInfoUI.SetActive(false);

        healthEffectCalculation(healthEffect);

        energyEffectCalculation(energyEffect);

    }


    private static void healthEffectCalculation(float healthEffect)
    {
        // --- Health --- //

        float healthBeforeConsumption = PlayerStateSystem.Instance.currentHealth;
        float maxHealth = PlayerStateSystem.Instance.maxHealth;

        if (healthEffect != 0)
        {
            if ((healthBeforeConsumption + healthEffect) > maxHealth)
            {
                PlayerStateSystem.Instance.setHealth(maxHealth);
            }
            else
            {
                PlayerStateSystem.Instance.setHealth(healthBeforeConsumption + healthEffect);
            }
        }
    }


    private static void energyEffectCalculation(float energyEffect)
    {
        // --- Energy --- //

        float energyBeforeConsumption = PlayerStateSystem.Instance.currentEnergy;
        float maxEnergy = PlayerStateSystem.Instance.maxEnergy;

        if (energyEffect != 0)
        {
            if ((energyBeforeConsumption + energyEffect) > maxEnergy)
            {
                PlayerStateSystem.Instance.setEnergy(maxEnergy);
            }
            else
            {
                PlayerStateSystem.Instance.setEnergy(energyBeforeConsumption + energyEffect);
            }
        }
    }
}