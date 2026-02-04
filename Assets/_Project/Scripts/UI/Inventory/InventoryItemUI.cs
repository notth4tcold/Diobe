using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField] private Image image;

    private InventoryItem item;
    private InventoryUI inventoryUI;
    private RectTransform rect;

    public void Init(InventoryItem item, InventoryUI ui) {
        this.item = item;
        inventoryUI = ui;
        rect = GetComponent<RectTransform>();

        image.sprite = item.data.icon;

        rect.sizeDelta = new Vector2(
            item.Width * inventoryUI.CellSize,
            item.Height * inventoryUI.CellSize
        );

        rect.anchoredPosition = inventoryUI.GridToScreen(item.x, item.y);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIUnequipItem);
        image.raycastTarget = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData) {
        rect.position = eventData.position + new Vector2(-rect.sizeDelta.x * 0.5f, rect.sizeDelta.y * 0.5f);

        Vector2 gridPosCheck = new Vector2(rect.position.x, rect.position.y) + new Vector2(inventoryUI.CellSize * 0.5f, -inventoryUI.CellSize * 0.5f);

        Vector2Int gridPos = inventoryUI.ScreenToGrid(gridPosCheck);
        if (gridPos.x >= 0 && gridPos.y >= 0) {
            inventoryUI.PreviewPlacement(item, gridPos);
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIEquipItem);
        image.raycastTarget = true;

        Vector2 gridPosCheck = new Vector2(rect.position.x, rect.position.y) + new Vector2(inventoryUI.CellSize * 0.5f, -inventoryUI.CellSize * 0.5f);

        Vector2Int gridPos = inventoryUI.ScreenToGrid(gridPosCheck);
        inventoryUI.TryPlaceItem(item, this, gridPos);

        inventoryUI.ClearPreview();
    }

    public void SnapToGrid(Vector2Int pos) {
        RectTransform cell = inventoryUI.GetCellRect(pos);
        if (cell == null) return;

        Vector2 topLeft = cell.anchoredPosition + new Vector2(-cell.rect.width * 0.5f, cell.rect.height * 0.5f);

        rect.anchoredPosition = topLeft;
    }
}
