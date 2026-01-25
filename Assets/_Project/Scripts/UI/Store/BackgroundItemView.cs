using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundItemView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button selectButton;     // кнопка на сам фон
    [SerializeField] private Button buyButton;        // кнопка покупки снизу
    [SerializeField] private TMP_Text priceText;      // цена
    [SerializeField] private Image checkmark;         // зеленая галочка
    [SerializeField] private Image buyButtonDim;      // затемнение (если есть отдельная картинка)
    [SerializeField] private Image previewImage;      // если нужно показать спрайт
    [SerializeField] private GameObject buyArea;


    private string id;
    private int price;
    private BackgroundStoreController controller;

    public void Bind(BackgroundStoreController controller, string id, int price, Sprite preview)
    {
        this.controller = controller;
        this.id = id;
        this.price = price;

        if (previewImage != null) previewImage.sprite = preview;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => controller.OnSelectPressed(id));

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => controller.OnBuyPressed(id));

        Refresh(); // первый рендер
    }

    public void Refresh()
    {
        var state = controller.GetStateFor(id, price);

        // галочка
        if (checkmark != null)
            checkmark.gameObject.SetActive(state.isSelected);

        // если куплено — скрываем покупку полностью
        if (buyArea != null)
            buyArea.SetActive(!state.isOwned);

        // если buyArea скрыт — дальше ничего не надо
        if (state.isOwned)
            return;

        // цена
        if (priceText != null)
        {
            priceText.text = price.ToString();
            priceText.color = state.canBuy ? Color.white : Color.red;
        }

        // купить можно только если хватает
        if (buyButton != null)
            buyButton.interactable = state.canBuy;

        // затемнение при нехватке
        if (buyButtonDim != null)
            buyButtonDim.gameObject.SetActive(!state.canBuy);
    }
}
