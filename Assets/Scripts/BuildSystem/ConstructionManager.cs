using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance { get; set; }

    //Building
    public GameObject itemToBeConstructed;
    public bool inConstructionMode = false;

    public GameObject constructionHoldingSpot;
    public GameObject ghostBank;
    public GameObject buildingBank;

    public bool isValidPlacement;
    public bool selectingAGhost;

    public GameObject selectedGhost;
    public GameObject player;

    public string itemToBeDestroyed;

    // Ghost
    public Material ghostSelectedMat;
    public Material ghostSemiTransparentMat;
    public Material ghostFullTransparentMat;

    public List<GameObject> allGhostsInExistence = new List<GameObject>();

    public List<Vector3> placedObjectPositions = new List<Vector3>();

    //UI
    public GameObject constructionUI;
    public GameObject workbenchChooseUI;
    public GameObject wallChooseUI;
    public GameObject floorChooseUI;
    public GameObject leftClickToBuildUI;

    [SerializeField] private int workbench_count = 0;
    [SerializeField] private int woods_count = 0;
    private TextMeshProUGUI workbench_availableText;
    private TextMeshProUGUI floor_availableText;
    private TextMeshProUGUI wall_availableText;
    private Image workbenchImage;
    private Image floorImage;
    private Image wallImage;

    [SerializeField] private int buildIndex;
    public int tempIndex; 

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
        buildingBank = PlayerBuildsManager.Instance.buildings;
        ghostBank = PlayerBuildsManager.Instance.buildingsGhost;

        workbench_availableText = constructionUI.transform.Find("Buildslot1_text").GetComponent<TextMeshProUGUI>();
        floor_availableText = constructionUI.transform.Find("Buildslot2_text").GetComponent<TextMeshProUGUI>();
        wall_availableText = constructionUI.transform.Find("Buildslot3_text").GetComponent<TextMeshProUGUI>();

        Transform buildSlot1 = constructionUI.transform.Find("Buildslot1");
        Transform buildSlot2 = constructionUI.transform.Find("Buildslot2");
        Transform buildSlot3 = constructionUI.transform.Find("Buildslot3");

        Transform workbench = buildSlot1.transform.Find("workbench");
        Transform wall = buildSlot2.transform.Find("wall");
        Transform floor = buildSlot3.transform.Find("floor");

        workbenchImage = workbench.GetComponent<Image>();
        wallImage = wall.GetComponent<Image>();
        floorImage = floor.GetComponent<Image>();
    }

    public void ActivateConstructionPlacement(string itemToConstruct)
    {
        CheckAvailable();

        GameObject item = Instantiate(Resources.Load<GameObject>("Buildable/" + itemToConstruct));

        //Change the name of the gameobject so it will not be (clone)
        item.name = itemToConstruct;

        item.transform.SetParent(constructionHoldingSpot.transform, false);
        itemToBeConstructed = item;
        itemToBeConstructed.gameObject.tag = "activeConstructable";

        // Disabling the non-trigger collider so our mouse can cast a ray
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = false;

        // Actiavting Construction mode
        inConstructionMode = true;

        //Check if player want to continue build
        buildIndex = tempIndex;
    }

    private void GetAllGhosts(GameObject itemToBeConstructed)
    {
        List<GameObject> ghostlist = itemToBeConstructed.gameObject.GetComponent<Constructable>().ghostList;

        foreach (GameObject ghost in ghostlist)
        {
            Debug.Log(ghost);
            ghost.transform.SetParent(ghostBank.transform);
            allGhostsInExistence.Add(ghost);
        }
    }

    public void PerformGhostDeletionScan()
    {
        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null)
            {
                if (ghost.GetComponent<GhostItem>().hasSamePosition == false) // if we did not already add a flag
                {
                    foreach (GameObject ghostX in allGhostsInExistence)
                    {
                        // First we check that it is not the same object
                        if (ghost.gameObject != ghostX.gameObject)
                        {
                            // If its not the same object but they have the same position
                            if (ghost != null && ghostX != null &&
                                XPositionToAccurateFloat(ghost.transform.position) == XPositionToAccurateFloat(ghostX.transform.position) && 
                                ZPositionToAccurateFloat(ghost.transform.position) == ZPositionToAccurateFloat(ghostX.transform.position) &&
                                YPositionToAccurateFloat(ghost.transform.position) == YPositionToAccurateFloat(ghostX.transform.position))
                            {
                                    // setting the flag
                                    ghostX.GetComponent<GhostItem>().hasSamePosition = true;
                                    break;
                            }

                        }

                    }
                    // Check against placed objects
                    foreach (Vector3 placedPosition in placedObjectPositions)
                    {
                        if (XPositionToAccurateFloat(ghost.transform.position) == XPositionToAccurateFloat(placedPosition) &&
                            YPositionToAccurateFloat(ghost.transform.position) == YPositionToAccurateFloat(placedPosition) &&
                            ZPositionToAccurateFloat(ghost.transform.position) == ZPositionToAccurateFloat(placedPosition))
                        {
                            ghost.GetComponent<GhostItem>().hasSamePosition = true;
                            break;
                        }
                    }
                }
            }
        }
        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null && ghost.GetComponent<GhostItem>().hasSamePosition)
            {
                    DestroyImmediate(ghost);
            }
        }
    }

    private float XPositionToAccurateFloat(Vector3 ghost)
    {
        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost;
            float pos = targetPosition.x;
            float xFloat = Mathf.Round(pos * 100f) / 100f;
            return xFloat;
        }
        return 0;
    }

    private float ZPositionToAccurateFloat(Vector3 ghost)
    {

        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost;
            float pos = targetPosition.z;
            float zFloat = Mathf.Round(pos * 100f) / 100f;
            return zFloat;

        }
        return 0;
    }

    private float YPositionToAccurateFloat(Vector3 ghost)
    {

        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost;
            float pos = targetPosition.y;
            float yFloat = Mathf.Round(pos * 100f) / 100f;
            return yFloat;

        }
        return 0;
    }

    private void Update()
    {
        if (inConstructionMode && InventorySystem.Instance.isOpen == false)
        {
            constructionUI.SetActive(true);
        }
        else
        {
            constructionUI.SetActive(false);
        }

        if (itemToBeConstructed != null && inConstructionMode)
        {
            if(itemToBeConstructed.name == "Floor" || itemToBeConstructed.name != "Wall")
            {
                if (CheckValidConstructionPosition())
                {
                    isValidPlacement = true;
                    itemToBeConstructed.GetComponent<Constructable>().SetValidColor();
                }
                else
                {
                    isValidPlacement = false;
                    itemToBeConstructed.GetComponent<Constructable>().SetInvalidColor();
                }
            }

            if(itemToBeConstructed.name == "Wall") 
            { 
                if (!CheckValidConstructionPosition())
                {
                    itemToBeConstructed.GetComponent<Constructable>().SetInvalidColor();
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var selectionTransform = hit.transform;
                if (selectionTransform.gameObject.CompareTag("ghost") && itemToBeConstructed.name == "Floor")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else if (selectionTransform.gameObject.CompareTag("wallGhost") && itemToBeConstructed.name == "Wall")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Ground")))
                {
                    itemToBeConstructed.SetActive(true);
                    selectingAGhost = false;
                    selectedGhost = null;

                    Vector3 newPosition = itemToBeConstructed.transform.position;
                    Quaternion newQuaternion = Quaternion.identity;
                    newPosition.y = hit.point.y;
                    itemToBeConstructed.transform.position = newPosition;
                    itemToBeConstructed.transform.rotation = newQuaternion;
                }
                else
                {
                    itemToBeConstructed.SetActive(true);
                    selectingAGhost = false;
                    selectedGhost = null;
                    itemToBeConstructed.transform.rotation = Quaternion.Euler(Vector3.zero);
                }
            }
        }

        // Left Mouse Click to Place item
        if (Input.GetMouseButtonDown(0) && inConstructionMode)
        {
            SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.buildSound);

            EquippableItemManager.Instance.animator.SetTrigger("build");

            //Floor
            if (isValidPlacement && selectedGhost == false && itemToBeConstructed.name == "Floor") 
            {
                PlaceItemFreeStyle();
                DestroyItem(itemToBeDestroyed);
                isValidPlacement = false;
                selectingAGhost = false;

                CheckAvailable();
                StartCoroutine(CheckIfPlayerWantToBuildMore());
            }
            //Objects can build on ground
            if (isValidPlacement && selectedGhost == false && 
                itemToBeConstructed.name != "Wall" && 
                itemToBeConstructed.name != "Floor")
            {
                PlaceItemFreeStyle();
                DestroyItem(itemToBeDestroyed);
                isValidPlacement = false;
                selectingAGhost = false;

                CheckAvailable();
                StartCoroutine(CheckIfPlayerWantToBuildMore());
            }
            //Objects can only be build on floor
            if (selectingAGhost)
            {
                PlaceItemInGhostPosition(selectedGhost);
                DestroyItem(itemToBeDestroyed);
                isValidPlacement = false;
                selectingAGhost = false;

                CheckAvailable();
                StartCoroutine(CheckIfPlayerWantToBuildMore());
            }
        }
        // Press X to Cancel                      
        if (Input.GetKeyDown(KeyCode.X) && isValidPlacement)
        {    
            CancelBuilding();
        }
    }

    void DestroyItem(string item)
    {
        InventorySystem.Instance.RemoveItem(item, 1);
        InventorySystem.Instance.RecalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
        StartCoroutine(Calculate());
    }

    //Build with Hammer
    public void CancelBuilding()
    {
        itemToBeDestroyed = null;
        DestroyImmediate(itemToBeConstructed);
        itemToBeConstructed = null;
    }

    public void ChooseBuilding(int index)
    {
        CheckAvailable();
        switch (index)
        {
            case 0:
                workbenchChooseUI.SetActive(true);
                wallChooseUI.SetActive(false);
                floorChooseUI.SetActive(false);
                CancelBuilding();
                if (workbench_count >= 1)
                {
                    leftClickToBuildUI.SetActive(true);
                    ActivateConstructionPlacement("Workbench");
                    itemToBeDestroyed = "Workbench";
                }
                break;
            case 1:
                workbenchChooseUI.SetActive(false);
                wallChooseUI.SetActive(true);
                floorChooseUI.SetActive(false);
                CancelBuilding();
                if (woods_count >= 1)
                {
                    leftClickToBuildUI.SetActive(true);
                    ActivateConstructionPlacement("Wall");
                    itemToBeDestroyed = "Wood";
                }
                break;
            case 2:
                workbenchChooseUI.SetActive(false);
                wallChooseUI.SetActive(false);
                floorChooseUI.SetActive(true);
                CancelBuilding();
                if (woods_count >= 1)
                {
                    leftClickToBuildUI.SetActive(true);
                    ActivateConstructionPlacement("Floor");
                    itemToBeDestroyed = "Wood";
                }
                break;
            default:
                leftClickToBuildUI.SetActive(false);
                break;
        }
    }

    //Checking available - UI
    public void CheckAvailable()
    {
        woods_count = 0;
        workbench_count = 0;

        foreach (string itemName in InventorySystem.Instance.itemList)
        {
            switch (itemName)
            {
                case "Workbench":
                    workbench_count += 1;
                    break;
                case "Wood":
                    woods_count += 1;
                    break;
                default:
                    break;
            }
        }

        workbench_availableText.text = "x" + workbench_count;
        floor_availableText.text = "x" + woods_count;
        wall_availableText.text = "x" + woods_count;

        if (workbench_count < 1)
        {
            SetImageAlpha(workbenchImage, 0.5f);
        }
        else
        {
            SetImageAlpha(workbenchImage, 1f);
        }

        if (woods_count < 1)
        {
            SetImageAlpha(floorImage, 0.5f);
            SetImageAlpha(wallImage, 0.5f);
        }
        else
        {
            SetImageAlpha(floorImage, 1f);
            SetImageAlpha(wallImage, 1f);
        }
    }

    void SetImageAlpha(Image image, float alpha)
    {
        Color tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }

    //Build & Preview
    private void PlaceItemInGhostPosition(GameObject copyOfGhost)
    {
        Vector3 ghostPosition = copyOfGhost.transform.position;
        Quaternion ghostRotation = copyOfGhost.transform.rotation;

        selectedGhost.gameObject.SetActive(false);

        // Setting the item to be active again (after disabled it in the ray cast)
        itemToBeConstructed.gameObject.SetActive(true);
        // Setting the parent to be the root of the scene
        itemToBeConstructed.transform.SetParent(transform.parent, true);

        itemToBeConstructed.transform.position = ghostPosition;
        itemToBeConstructed.transform.rotation = ghostRotation;

        // Enabling back the solider collider that disabled earlier
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;

        // Setting the default color/material
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();

        if (itemToBeConstructed.name == "Floor")
        {
            // Making the Ghost Children to no longer be children of this item
            itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();
            itemToBeConstructed.tag = "placedBuilding";

            //Adding all the ghosts of this item into the manager's ghost bank
            GetAllGhosts(itemToBeConstructed);
            PerformGhostDeletionScan();

            //Tranfer to buildings bank
            itemToBeConstructed.transform.SetParent(buildingBank.transform);
        }
        else if (itemToBeConstructed.name == "Wall")
        {
            itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();
            itemToBeConstructed.tag = "placedWall";

            GetAllGhosts(itemToBeConstructed);
            PerformGhostDeletionScan();

            itemToBeConstructed.transform.SetParent(buildingBank.transform);
        }

        placedObjectPositions.Add(itemToBeConstructed.transform.position);

        itemToBeConstructed = null;
    }

    private void PlaceItemFreeStyle()
    {
        if (itemToBeConstructed.name == "Floor")
        {
            // Setting the parent to be the root of our scene
            itemToBeConstructed.transform.SetParent(transform.parent, true);

            // Making the Ghost Children to no longer be children of this item
            itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();
            // Setting the default color/material
            itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();
            itemToBeConstructed.tag = "placedBuilding";
            itemToBeConstructed.GetComponent<Constructable>().enabled = false;
            // Enabling back the solider collider that we disabled earlier
            itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;

            //Adding all the ghosts of this item into the manager's ghost bank
            GetAllGhosts(itemToBeConstructed);
            PerformGhostDeletionScan();

            itemToBeConstructed.transform.SetParent(buildingBank.transform);
        }
        else if (itemToBeConstructed.name != "Wall" && itemToBeConstructed.name != "Floor")
        {
            itemToBeConstructed.transform.SetParent(transform.parent, true);
            itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();
            itemToBeConstructed.tag = "placedBuilding";
            itemToBeConstructed.GetComponent<Constructable>().enabled = false;
            itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;

            itemToBeConstructed.transform.SetParent(buildingBank.transform);
        }
        placedObjectPositions.Add(itemToBeConstructed.transform.position);

        itemToBeConstructed = null;
    }

    private bool CheckValidConstructionPosition()
    {
        if (itemToBeConstructed != null)
        {
            return itemToBeConstructed.GetComponent<Constructable>().isValidToBeBuilt;
        }

        return false;
    }

    //Loading buildings
    public void LoadBuildings(GameObject buildings)
    {
        itemToBeConstructed = buildings;
        itemToBeConstructed.gameObject.tag = "activeConstructable";

        // Disabling the non-trigger collider so our mouse can cast a ray
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = false;

        // Actiavting Construction mode
        inConstructionMode = true;

        //Check if player want to continue build
        buildIndex = tempIndex;

        //Floor
        if (itemToBeConstructed.name == "Floor")
        {
            PlaceItemFreeStyle();
            isValidPlacement = false;
            selectingAGhost = false;
        }
        //Objects can build on ground
        if (itemToBeConstructed.name != "Wall" &&
            itemToBeConstructed.name != "Floor")
        {
            PlaceItemFreeStyle();
            isValidPlacement = false;
            selectingAGhost = false;
        }
        //Objects can only be build on floor
        if (itemToBeConstructed.name == "Wall")
        {
            PlaceItemInGhostPosition(selectedGhost);
            isValidPlacement = false;
            selectingAGhost = false;
        }
    }

    public IEnumerator Calculate()
    {
        yield return 0;
        InventorySystem.Instance.RecalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }
    public IEnumerator CheckIfPlayerWantToBuildMore()
    {
        yield return 0;
        Debug.Log($"inConstructionMode: {inConstructionMode}, buildIndex: {buildIndex}, tempIndex: {tempIndex}");
        if (inConstructionMode && buildIndex == tempIndex)
        {
            CheckAvailable();
            ChooseBuilding(buildIndex);
        }
    }
}
