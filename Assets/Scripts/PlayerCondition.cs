using System;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public event Action OnTakeDamage;

    public UICondition UICondition { get; set; }

    private Condition Health => UICondition.Health;
    private Condition Hunger => UICondition.Hunger;
    private Condition Stamina => UICondition.Stamina;

    [SerializeField] private float noHungerHealthDecay;

    private void Update()
    {
        Hunger.Subtract(Hunger.PassiveValue * Time.deltaTime);
        Stamina.Add(Stamina.PassiveValue * Time.deltaTime);

        if (Hunger.CurValue < 0f)
        {
            Health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (Health.CurValue < 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        Health.Add(amount);
    }

    public void Eat(float amount)
    {
        Hunger.Add(amount);
    }

    public void Die()
    {
        Debug.Log("플레이어가 사망했습니다.");
    }
}