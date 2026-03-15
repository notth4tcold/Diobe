
using UnityEngine.EventSystems;

public interface IItemContainerUI {
    void OnBeginDrag(ItemUI itemUI, PointerEventData eventData);
    void OnDrag(ItemUI itemUI, PointerEventData eventData);
    bool CanReceiveItem(ItemUI itemUI, PointerEventData eventData);
    void ReceiveItem(ItemUI itemUI, PointerEventData eventData);
    void CancelDrag(ItemUI itemUI, PointerEventData eventData);
    void DetachItem(ItemUI itemUI);
    void RemoveItem(ItemUI itemUI);
    void ShowVisualState(ItemUI itemUI, PointerEventData eventData);
    void ClearVisualState(ItemUI itemUI);
}
