using UnityEngine;
using UnityEngine.UI;

public class InventoryCellUI : MonoBehaviour {

    [SerializeField] private Image background;

    public int Index { get; private set; }

    public void SetIndex(int index) {
        Index = index;
    }

    public void SetHighlight(Color color) {
        background.color = color;
    }

    public void ClearHighlight() {
        background.color = Color.white;
    }
}
