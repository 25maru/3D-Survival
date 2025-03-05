using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    [SerializeField] private float curValue;
    [SerializeField] private float maxValue;
    [SerializeField] private float startValue;
    [SerializeField] private float passiveValue;
    [SerializeField] private Image uIBar;

    public float CurValue => curValue;
    public float PassiveValue => passiveValue;

    private void Start()
    {
        curValue = startValue;
    }

    private void Update()
    {
        uIBar.fillAmount = GetPercentage();
    }

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}