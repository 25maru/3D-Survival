using UnityEngine;
using cakeslice;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void SetOutline(bool show);
    public bool OnInteract();
}

public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData data;
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
        //Player 스크립트 먼저 수정
        CharacterManager.Instance.Player.ItemData = data;
        CharacterManager.Instance.Player.AddItem?.Invoke();
        Destroy(gameObject);

        return true;
    }
}