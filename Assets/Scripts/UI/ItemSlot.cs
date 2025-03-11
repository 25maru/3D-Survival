using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [Header("Slot")]
    [SerializeField] private Button button;
    [SerializeField] private Image icon;

    [Header("Data")]
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private GameObject equipMark;

    public Button Button => button;

    public UIInventory Inventory { get; set; }
    public ItemData Item { get; set; }
    public bool Equipped;
    public int Quantity { get; set; }
    public int Index { get; set; }


    public TextMeshProUGUI IndexText => indexText;

    private void OnEnable()
    {
        equipMark.SetActive(Equipped);
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = Item.icon;
        quantityText.text = Quantity > 1 ? Quantity.ToString() : string.Empty;

        equipMark.SetActive(Equipped);
    }

    public void Clear()
    {
        Item = null;
        icon.gameObject.SetActive(false);
        quantityText.text = string.Empty;
    }

    public void OnClickButton()
    {
        Inventory.SelectItem(Index);
    }
}