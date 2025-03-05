using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerCondition condition;

    public PlayerController Controller => controller;
    public PlayerCondition Condition => condition;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
    }
}