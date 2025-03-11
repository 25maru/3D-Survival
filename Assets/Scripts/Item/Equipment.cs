using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    [SerializeField] private Transform equipParent;

    private PlayerController controller;
    private PlayerCondition condition;

    public Equip CurEquip { get; set; }

    void Start()
    {
        controller = CharacterManager.Instance.Player.Controller;
        condition = CharacterManager.Instance.Player.Condition;
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();
        CurEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if (CurEquip != null)
        {
            Destroy(CurEquip.gameObject);
            CurEquip = null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && CurEquip != null && controller.CanLook)
        {
            CurEquip.OnAttackInput();
        }
    }
}