using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlotButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject selectedMark; // рамка/галочка/подсветка

    private int _index;
    private System.Action<int> _onClick;

    public void Init(int index, System.Action<int> onClick)
    {
        _index = index;
        _onClick = onClick;

        if (button == null) button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _onClick?.Invoke(_index));
    }

    public void SetNumber(int number) => label.text = number.ToString();

    public void SetSelected(bool selected)
    {
        if (selectedMark != null) selectedMark.SetActive(selected);
    }
}
