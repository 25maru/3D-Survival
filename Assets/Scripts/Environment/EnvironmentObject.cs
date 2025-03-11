using UnityEngine;
using cakeslice;

public class EnvironmentObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected EnvironmentData data;
    [SerializeField] protected Outline outline;

    public string GetInteractPrompt()
    {
        string str = $"<font=\"GmarketSansMedium SDF\" material=\"GmarketSansMedium SDF Glow Blue\">{data.displayName}</font> - {data.description}";
        return str;
    }

    public virtual void SetOutline(bool show)
    {
        outline.color = show ? 0 : 1;
    }

    public virtual bool OnInteract()
    {
        return false;
    }
}