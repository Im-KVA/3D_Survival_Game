using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructable : MonoBehaviour
{
    // Validation
    public bool isGrounded;
    public bool isOverlappingItems;
    public bool isValidToBeBuilt;
    public bool detectedGhostMemeber;

    // Material related
    private Renderer mRenderer;
    public Material redMaterial;
    public Material greenMaterial;
    public Material defaultMaterial;

    public List<GameObject> ghostList = new List<GameObject>();

    public BoxCollider solidCollider; // We need to drag this collider manualy into the inspector

    private void Start()
    {
        mRenderer = GetComponent<Renderer>();

        mRenderer.material = defaultMaterial;
        foreach (Transform child in transform)
        {
            ghostList.Add(child.gameObject);
        }

    }
    void Update()
    {
        if (isGrounded && isOverlappingItems == false)
        {
            isValidToBeBuilt = true;
        }
        else
        {
            isValidToBeBuilt = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && gameObject.CompareTag("activeConstructable"))
        {
            isGrounded = true;
        }

        if (other.CompareTag("Tree") || other.CompareTag("pickable") && gameObject.CompareTag("activeConstructable"))
        {

            isOverlappingItems = true;
        }

        if (other.gameObject.CompareTag("ghost") && gameObject.CompareTag("activeConstructable"))
        {
            detectedGhostMemeber = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground") && gameObject.CompareTag("activeConstructable"))
        {
            isGrounded = false;
        }

        if (other.CompareTag("Tree") || other.CompareTag("pickable") && gameObject.CompareTag("activeConstructable"))
        {
            isOverlappingItems = false;
        }

        if (other.gameObject.CompareTag("ghost") && gameObject.CompareTag("activeConstructable"))
        {
            detectedGhostMemeber = false;
        }
    }

    public void SetInvalidColor()
    {
        if (mRenderer != null)
        {
            mRenderer.material = redMaterial;
        }
    }

    public void SetValidColor()
    {
        mRenderer.material = greenMaterial;
    }

    public void SetDefaultColor()
    {
        mRenderer.material = defaultMaterial;
    }

    public void ExtractGhostMembers()
    {
        foreach (GameObject item in ghostList)
        {
            item.transform.SetParent(transform.parent, true);
            //  item.gameObject.GetComponent<GhostItem>().solidCollider.enabled = false;
            item.gameObject.GetComponent<GhostItem>().isPlaced = true;
        }
    }
}
