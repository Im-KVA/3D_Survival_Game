using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance { get; set; }

    public GameObject interaction_Info_UI;
    public GameObject dotIcon;
    public GameObject handIcon;

    public bool canPickupItem;

    public GameObject selectedTree;
    public GameObject chopHolder;

    public GameObject selectedStorageBox;

    public LayerMask layerMask;

    TextMeshProUGUI interaction_text;

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
        interaction_text = interaction_Info_UI.transform.Find("interaction_text").GetComponent<TextMeshProUGUI>();
        layerMask = LayerMask.GetMask("Ignore Raycast");
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10, ~layerMask))
        {
            var selectionTransform = hit.transform;

            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
            InteractableObject interactableObject = selectionTransform.GetComponent<InteractableObject>();
            Creatures creatures = selectionTransform.GetComponent<Creatures>();

            //Information_UI
            if (interactableObject)
            {
                Debug.Log("InteractableObject found");
                canPickupItem = false;
                interaction_text.text = interactableObject.GetItemName();
                interaction_Info_UI.SetActive(true);
                
                //Items & Creatures
                if (interactableObject.IsDestroyable()) 
                {
                    canPickupItem = true;
                    dotIcon.SetActive(false);
                    handIcon.SetActive(true);
                    
                    //Pick up items
                    if (Input.GetMouseButtonDown(0) && !InventorySystem.Instance.CheckIfFull())
                    {
                        SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.itemSound);
                        InventorySystem.Instance.AddToInventory(interactableObject.GetItemName());
                        Destroy(selectionTransform.gameObject);
                    }
                }
                else { canPickupItem = false; }
            }
            else 
            {
                interaction_Info_UI.SetActive(false);
                canPickupItem = false;

                dotIcon.SetActive(true);
                handIcon.SetActive(false);
            }

            //Storage Box
            StorageBox box = selectionTransform.GetComponent<StorageBox>();

            if (box && box.playerInRange)
            {
                selectedStorageBox = box.gameObject;

                if (Input.GetMouseButton(0))
                {
                    StorageSystem.Instance.OpenBox(box);
                }
            }
            else
            {
                if (selectedStorageBox != null)
                {
                    selectedStorageBox = null;
                }
            }

            //Attacking & Looting
            if (creatures && creatures.playerInRange)
            {
                if (creatures.isDead)
                {
                    dotIcon.SetActive(false);
                    handIcon.SetActive(true);

                    if (Input.GetMouseButtonDown(0))
                    {
                        Looting lootable = creatures.GetComponent<Looting>();
                        SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.lootingSound);
                        Loot(lootable);
                        Destroy(creatures.gameObject);
                    } 
                }
                else if (Input.GetMouseButtonDown(0) && ToolsDmg.Instance.CheckHoldingAttackableTool())
                {
                    dotIcon.SetActive(true);
                    handIcon.SetActive(false);

                    ToolsDmg.Instance.animator.SetTrigger("attack");
                    StartCoroutine(DealDmgTo(creatures, 0.6f, ToolsDmg.Instance.GetToolsDmg()));
                }
            }

            //Chopping Tree
            if (choppableTree && choppableTree.playerInRange)
            {
                dotIcon.SetActive(true);
                handIcon.SetActive(false);
                choppableTree.canBeChopped = true;
                selectedTree = choppableTree.gameObject;
                chopHolder.gameObject.SetActive(true);
            }
            else
            {
                if (selectedTree != null)
                {
                    selectedTree.gameObject.GetComponent<ChoppableTree>().canBeChopped = false;
                    selectedTree = null;
                    chopHolder.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            interaction_Info_UI.SetActive(false);
        }
    }

    //Looting function
    public void Loot(Looting lootable)
    {
        List<LootingFinal> finals = new List<LootingFinal>();

        if (!lootable.lootingCalculated)
        {
            foreach (LootingBank loot in lootable.lootingBank)
            {
                var lootAmount = UnityEngine.Random.Range(loot.min, loot.max + 1); //min 0; max 5
                if (lootAmount != 0)
                {
                    LootingFinal temp = new LootingFinal();
                    temp.item = loot.item;
                    temp.amount = lootAmount;

                    finals.Add(temp);
                }
            }

            lootable.finalLoot = finals;
            lootable.lootingCalculated = true;
        }

        Vector3 lootSpawnPosition = lootable.gameObject.transform.position;

        foreach (LootingFinal lootingFinal in lootable.finalLoot)
        {
            for (int i = 0; i < lootingFinal.amount; i++)
            {
                GameObject lootSpawn = Instantiate(Resources.Load<GameObject>("Items_model/" + lootingFinal.item.name + "_Model"),
                    new Vector3(lootSpawnPosition.x, lootSpawnPosition.y + 0.2f, lootSpawnPosition.z),
                    Quaternion.Euler(0,0,0));
            }
        }
    }

    IEnumerator DealDmgTo(Creatures creatures, float delay, int damage)
    {
        //Debug.Log("Take dmg");
        yield return new WaitForSeconds(delay);
        SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.useAxeSound);
        creatures.TakeDmg(damage);
    }

    public void DisableSelection()
    {
        dotIcon.SetActive(false);
        interaction_Info_UI.SetActive(false);
    }

    public void EnableSelection()
    {
        dotIcon.SetActive(true);
        interaction_Info_UI.SetActive(true);
    }
}
