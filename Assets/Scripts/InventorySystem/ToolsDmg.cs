using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsDmg : MonoBehaviour
{
    [SerializeField] private int toolsDmg = -1;
    public static ToolsDmg Instance { get; set; }
    public Animator animator;

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

    public int GetToolsDmg()
    {
        return toolsDmg;
    }

    public bool CheckHoldingAttackableTool()
    {
        if (QuickSlotSystem.Instance.selectedItem != null)
        {
            if (toolsDmg >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
