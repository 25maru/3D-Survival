using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action AddItem;

    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerCondition condition;
    [SerializeField] private ItemData itemData;

    public PlayerController Controller => controller;
    public PlayerCondition Condition => condition;
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