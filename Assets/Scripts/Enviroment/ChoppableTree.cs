using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChoppableTree : MonoBehaviour
{
    public bool playerInRange;
    public bool canBeChopped;

    public float treeMaxHealth = 3;
    public float treeHealth;

    public Animator animator;

    public float energySpentChoppingWood = 20;

    private void Start()
    {
        treeHealth = treeMaxHealth;
        animator = transform.parent.transform.parent.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GetHit()
    {
        StartCoroutine(hit());
    }

    public IEnumerator hit()
    {
        yield return new WaitForSeconds(0.6f);

        SoundManageSystem.Instance.PlaySound(SoundManageSystem.Instance.useAxeSound);

        animator.SetTrigger("shake");
        treeHealth -= 1;

        PlayerStateSystem.Instance.currentEnergy -= energySpentChoppingWood;

        if(treeHealth <= 0)
        {
            TreeIsChopped();
        }
    }

    void TreeIsChopped()
    {
        var tree = transform.parent.transform.parent;

        canBeChopped = false;
        ActionManager.Instance.selectedTree = null;
        ActionManager.Instance.chopHolder.gameObject.SetActive(false);

        TreeManageSystem.Instance.TreeFalling(tree.gameObject.name, tree.position, tree.rotation);
        Destroy(tree.gameObject);
    }

    private void Update()
    {
        if (playerInRange) 
        {
            GlobalState.Instance.resourceHealth = treeHealth;
            GlobalState.Instance.resourceMaxHealth = treeMaxHealth;
        }
    }
}
