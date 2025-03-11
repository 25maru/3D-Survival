using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action AddItem;

    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerCondition condition;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private Equipment equipment;

    public PlayerController Controller => controller;
    public PlayerCondition Condition => condition;
    public Transform DropPosition => dropPosition;
    public Equipment Equipment => equipment;
    public ItemData ItemData { get; set; }

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
    }
}