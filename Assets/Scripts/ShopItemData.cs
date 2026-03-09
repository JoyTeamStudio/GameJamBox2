using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Scriptable Objects/ShopItemData")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    public string displayName;

    [TextArea(3, 20)] public string description;
    public Sprite icon;

    public int price;
}
