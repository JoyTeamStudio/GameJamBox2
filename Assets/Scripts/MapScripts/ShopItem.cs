using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, ISelectHandler
{
    [HideInInspector] public ShopItemData data;

    public Image image;
    public TextMeshProUGUI priceText;
    public ShopMerchant merchant;
    public int itemIndex;

    public void SetItem(ShopItemData data, ShopMerchant merchant, int itemIndex)
    {
        this.data = data;
        this.merchant = merchant;
        this.itemIndex = itemIndex;

        SetItemData();
    }

    public void SetItemData()
    {
        image.sprite = data.icon;
        priceText.text = data.price.ToString();
    }

    public void Display()
    {
        if(MainManager.Instance.money >= data.price)
            FindAnyObjectByType<GameManager>().ConfirmPurchasePopUp(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        FindAnyObjectByType<GameManager>().DisplayShopItem(data);
    }
}
