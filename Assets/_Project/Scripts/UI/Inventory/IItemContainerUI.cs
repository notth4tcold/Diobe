
using UnityEngine.EventSystems;

public interface IItemContainerUI {
    void OnBeginDrag(InventoryItemUI itemUI, PointerEventData eventData);
    void OnDrag(InventoryItemUI itemUI, PointerEventData eventData);
    bool CanReceiveItem(InventoryItemUI itemUI, PointerEventData eventData);
    void ReceiveItem(InventoryItemUI itemUI, PointerEventData eventData);
    void CancelDrag(InventoryItemUI itemUI, PointerEventData eventData);
    void DetachItem(InventoryItemUI itemUI);
    void RemoveItem(InventoryItemUI itemUI);
    void ShowVisualState(InventoryItemUI itemUI, PointerEventData eventData);
    void ClearVisualState(InventoryItemUI itemUI);
}
