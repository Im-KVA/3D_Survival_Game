using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManageSystem : MonoBehaviour
{
    public static TreeManageSystem Instance { get; set; }

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

    public void TreeFalling(string treeName, Vector3 position, Quaternion rotation)
    {
        //Debug.Log(treeName);
        string name = treeName;
        string other = "(Clone)";
        string result = name.Replace(other, "");

        GameObject deadTree = Instantiate(Resources.Load<GameObject>("Tree/" + result + "_Dead"), position, rotation);

        var treeWood = deadTree.transform.Find("Birch_top");

        StartCoroutine(TreeFallDown(treeWood));
    }

    public IEnumerator TreeFallDown(Transform treeWood)
    {
        yield return new WaitForSeconds(5f);
        
        //Debug.Log("Tree fall down");

        GameObject choppedTreeParent = Instantiate(Resources.Load<GameObject>("Tree/" + "Chopped_Tree"), treeWood.position, treeWood.rotation);
        Destroy(treeWood.gameObject);
        
        choppedTreeParent.transform.DetachChildren();
        Destroy(choppedTreeParent.gameObject);
    }
}
