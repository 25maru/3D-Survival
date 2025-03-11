using UnityEngine;
using cakeslice;

public class EnvironmentObject : MonoBehaviour, IInteractable
{
    [SerializeField] private EnvironmentData data;
    [SerializeField] private Outline outline;

    public string GetInteractPrompt()
    {
        string str = $"<font=\"GmarketSansMedium SDF\" material=\"GmarketSansMedium SDF Glow Blue\">{data.displayName}</font> - {data.description}";
        return str;
    }

    public void SetOutline(bool show)
    {
        outline.color = show ? 0 : 1;
    }

    public bool OnInteract()
    {
        return false;
    }
}