using UnityEngine;

public enum EnvironmentType
{
    Interactable,
    NonInteractable
}

[CreateAssetMenu(fileName = "Environment", menuName = "Scriptable Object/Environment Data", order = 1)]
public class EnvironmentData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public EnvironmentType type;
}