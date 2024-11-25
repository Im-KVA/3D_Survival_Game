using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostItem : MonoBehaviour
{
    public BoxCollider solidCollider; // Set manually

    public Renderer mRenderer;
    private Material semiTransparentMat; // Used for debug - insted of the full trasparent
    private Material fullTransparentMat;
    private Material selectedMaterial;

    public bool isPlaced;

    // A flag for the deletion algorithm
    public bool hasSamePosition = false;
    private void Start()
    {
        mRenderer = GetComponent<Renderer>();

        semiTransparentMat = ConstructionManager.Instance.ghostSemiTransparentMat;
        fullTransparentMat = ConstructionManager.Instance.ghostFullTransparentMat;
        selectedMaterial = ConstructionManager.Instance.ghostSelectedMat;

        mRenderer.material = fullTransparentMat; //Change to semi if in debug else full

        solidCollider.enabled = false;
    }

    private void Update()
    {
        if (ConstructionManager.Instance.inConstructionMode)
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), ConstructionManager.Instance.player.GetComponent<Collider>());
        }

        if (ConstructionManager.Instance.inConstructionMode && isPlaced)
        {
            solidCollider.enabled = true;
        }

        if (!ConstructionManager.Instance.inConstructionMode)
        {
            solidCollider.enabled = false;
        }

        // Triggering the material
        if (ConstructionManager.Instance.selectedGhost == gameObject)
        {
            mRenderer.material = selectedMaterial;
        }
        else
        {
            mRenderer.material = fullTransparentMat; //Change to semi if in debug else full
        }
    }
}
