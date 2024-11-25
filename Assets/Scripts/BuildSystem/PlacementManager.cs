using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance { get; set; }

    public GameObject placementHoldingSpot;
    public GameObject enviromentPlaceables;

    public bool inPlacementMode;

    [SerializeField] bool isValidPlacement;
    [SerializeField] GameObject placementUI;

    public GameObject itemToBePlaced;
    public GameObject inventoryItemToDestroy;

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

    public void Start()
    {
        enviromentPlaceables = PlayerBuildsManager.Instance.placements;
    }

    public void ActivatePlacementMode(string itemToPlace)
    {
        GameObject item = Instantiate(Resources.Load<GameObject>("Buildable/" + itemToPlace));

        // Changing the name of the gameobject so it will not be (clone)
        item.name = itemToPlace;

        // Setting the item to be a child of our placement holding spot
        item.transform.SetParent(placementHoldingSpot.transform, false);

        // Saving a reference to the item we want to place
        itemToBePlaced = item;

        // Actiavting Construction mode
        inPlacementMode = true;
    }



    private void Update()
    {
        if (inPlacementMode)
        {
            placementUI.SetActive(true);
        }
        else
        {
            placementUI.SetActive(false);
        }

        if (itemToBePlaced != null && inPlacementMode)
        {
            if (IsCheckValidPlacement())
            {
                isValidPlacement = true;
                itemToBePlaced.GetComponent<PlaceableItem>().SetValidColor();
            }
            else
            {
                isValidPlacement = false;
                itemToBePlaced.GetComponent<PlaceableItem>().SetInvalidColor();
            }
        }

        // Left Mouse Click to Place item
        if (Input.GetMouseButtonDown(0) && inPlacementMode && isValidPlacement)
        {
            SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.buildSound);

            PlaceItemFreeStyle();
            DestroyItem(inventoryItemToDestroy);
        }

        // Cancel Placement             
        if (Input.GetKeyDown(KeyCode.X))
        {
            inventoryItemToDestroy.SetActive(true);
            inventoryItemToDestroy = null;
            DestroyItem(itemToBePlaced);
            itemToBePlaced = null;
            inPlacementMode = false;
        }
    }

    private bool IsCheckValidPlacement()
    {
        if (itemToBePlaced != null)
        {
            return itemToBePlaced.GetComponent<PlaceableItem>().isValidToBeBuilt;
        }

        return false;
    }

    private void PlaceItemFreeStyle()
    {
        // Setting the parent to be the root of our scene
        itemToBePlaced.transform.SetParent(enviromentPlaceables.transform, true);

        // Setting the default color/material
        itemToBePlaced.GetComponent<PlaceableItem>().SetDefaultColor();
        itemToBePlaced.GetComponent<PlaceableItem>().enabled = false;

        itemToBePlaced = null;

        inPlacementMode = false;
    }

    private void DestroyItem(GameObject item)
    {
        DestroyImmediate(item);
        InventorySystem.Instance.RecalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }
}
