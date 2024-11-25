using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableItem : MonoBehaviour
{
    // Validation
    [SerializeField] bool isGrounded;
    [SerializeField] bool isOverlappingItems;
    public bool isValidToBeBuilt;

    [SerializeField] BoxCollider solidCollider;
    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
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

        // Raycast from the box's position towards its center

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("Ground")))
        {
            isGrounded = true;

            var item = PlacementManager.Instance.itemToBePlaced;
            
            item.SetActive(true);

            Vector3 newPosition = item.transform.position;
            Quaternion newQuaternion = Quaternion.identity;

            newPosition.y = hit.point.y;
            item.transform.position = newPosition;
            item.transform.rotation = newQuaternion;
        }
        else
        {
            isGrounded = false;
        }
    }

    #region || --- On Triggers --- |
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && PlacementManager.Instance.inPlacementMode)
        {
            // Making sure the item is parallel to the ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Align the box's rotation with the ground normal
                Quaternion newRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                transform.rotation = newRotation;

                isGrounded = true;
            }
        }

        if (other.CompareTag("Tree") || other.CompareTag("pickable"))
        {
            isOverlappingItems = true;
        }
    }
    #endregion

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground") && PlacementManager.Instance.inPlacementMode)
        {
            isGrounded = false;
        }

        if (other.CompareTag("Tree") || other.CompareTag("pickable") && PlacementManager.Instance.inPlacementMode)
        {
            isOverlappingItems = false;
        }
    }

    #region || --- Set Outline Colors --- |
    public void SetInvalidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red;
        }

    }

    public void SetValidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
        }
    }

    public void SetDefaultColor()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
    #endregion
}
