using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquippableItemManager : MonoBehaviour
{
    public static EquippableItemManager Instance { get; set; }

    public Animator animator;

    //Attacking
    public bool isHoldingAttackableTool = false;

    [SerializeField]
    private bool isCooldown = false;
    private int index;

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
        animator = GetComponent<Animator>();
        
    }

    private void Update()
    {
        if (QuickSlotSystem.Instance.selectedItem != null)
        {
            string inHandName = QuickSlotSystem.Instance.selectedItem.name;

            // Axe
            if (inHandName == "Axe(Clone)" || inHandName == "Axe") //For testing
            {
                GameObject selectedTree = ActionManager.Instance.selectedTree;
                if (selectedTree != null)
                {
                    if (Input.GetMouseButtonDown(0) &&
                    InventorySystem.Instance.isOpen == false &&
                    ActionManager.Instance.canPickupItem == false &&
                    isCooldown == false)
                    {
                        //Debug.Log("Chop Tree");
                        selectedTree.GetComponent<ChoppableTree>().GetHit();

                        animator.SetTrigger("hit");
                        StartCoroutine(HitCooldown());
                    }
                }
            }
            //Pickaxe

            // Hammer
            if (inHandName == "Hammer(Clone)" && !PlacementManager.Instance.inPlacementMode)
            {
                ConstructionManager.Instance.inConstructionMode = true;
                ConstructionManager.Instance.CheckAvailable();

                if (Input.GetKeyDown(KeyCode.Q) &&
                    InventorySystem.Instance.isOpen == false)
                {
                    animator.SetTrigger("rotate");

                    ConstructionManager.Instance.tempIndex = index;
                    ConstructionManager.Instance.ChooseBuilding(index);

                    SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.changeBuildingSound);

                    if (index < 2) 
                    { 
                        index += 1; 
                    } 
                    else
                    {
                        index = 0;
                    }
                }
            }
            else
            {
                index = -1;
                ConstructionManager.Instance.tempIndex = index;
                ConstructionManager.Instance.inConstructionMode = false;
                ConstructionManager.Instance.CancelBuilding();
            }
        }
    }

    private IEnumerator HitCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(1f);
        isCooldown = false;
    }
}
