using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : EnvironmentObject
{
    [SerializeField] private Animator animator;
    [SerializeField] private UIInventory uIInventory;
    [SerializeField] private ItemData key;
    private bool isOpen = false;

    private void Update()
    {
        if (outline.color == 1) return;

        if (HasKey() && outline.color == 2)
        {
            outline.color = 0;
        }

        if (!HasKey() && outline.color == 0)
        {
            outline.color = 2;
        }
    }

    public override void SetOutline(bool show)
    {
        outline.color = show ? (HasKey() ? 0 : 2) : 1;
    }

    public override bool OnInteract()
    {
        if (HasKey())
        {
            isOpen = !isOpen;
            animator.SetBool("IsOpen", isOpen);
        }

        return false;
    }

    private bool HasKey()
    {
        return Equals(uIInventory.ItemData, key);
    }
}
