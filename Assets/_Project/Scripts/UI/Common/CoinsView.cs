using TMPro;
using UnityEngine;

public class CoinsView : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;

    private CoinWallet wallet;

    private void Awake()
    {
        if (coinsText == null)
            coinsText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        wallet = CoinWallet.Instance;

        if (coinsText == null)
        {
            Debug.LogError($"[CoinsView] coinsText missing on {name}", this);
            return;
        }

        if (wallet == null)
        {
            Debug.LogError("[CoinsView] CoinWallet.Instance not found. CoinWallet must exist in the first loaded scene and be DontDestroyOnLoad.");
            return;
        }

        wallet.CoinsChanged += UpdateView;
        UpdateView(wallet.Data?.coins ?? 0);
    }

    private void OnDisable()
    {
        if (wallet != null)
            wallet.CoinsChanged -= UpdateView;
    }

    private void UpdateView(int coins)
    {
        coinsText.text = CoinFormat.Format(coins);
    }
}
