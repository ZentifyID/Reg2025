using UnityEngine;
using UnityEngine.UI;

public class BuyPackButton : MonoBehaviour
{
    [SerializeField] private StoreWindowController store;
    [SerializeField] private int coinsAmount;
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(() => store.BuyCoinPack(coinsAmount));
    }
}
