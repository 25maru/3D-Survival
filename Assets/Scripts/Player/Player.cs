using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action AddItem;

    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerCondition condition;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private ItemData itemData;

    public PlayerController Controller => controller;
    public PlayerCondition Condition => condition;
    public Transform DropPosition => dropPosition;
    public ItemData ItemData
    {
        get => itemData;
        set => itemData = value;
    }

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
    }
}