using UnityEngine;

public class UICondition : MonoBehaviour
{
    [SerializeField] private Condition health;
    [SerializeField] private Condition hunger;
    [SerializeField] private Condition stamina;

    public Condition Health => health;
    public Condition Hunger => hunger;
    public Condition Stamina => stamina;

    private void Start()
    {
        CharacterManager.Instance.Player.Condition.UICondition = this;
    }
}