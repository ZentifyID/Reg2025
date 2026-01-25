using UnityEngine;

public class SelectItemButton : MonoBehaviour
{
    [SerializeField] private AssemblyController controller;
    [SerializeField] private ItemType itemType;

    public void Click()
    {
        controller.SelectItem(itemType);
    }
}
