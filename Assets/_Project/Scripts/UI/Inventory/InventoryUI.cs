using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour, IItemContainerUI {
    [Header("References")]
    [SerializeField] private Transform itemsParent;
    [SerializeField] private InventoryItemUI itemPrefab;
    [SerializeField] private InventoryCellUI[] cells;

    private Dictionary<InventoryItem, InventoryItemUI> itemToUI = new();
    private InventoryGrid inventoryGrid;
    private bool isReceivingItem;
    private Vector2 defaultCellSize;

    void Awake() {
        defaultCellSize = ((RectTransform)cells[0].transform).sizeDelta;
    }

    void Initialize() {
        int totalCells = inventoryGrid.Width * inventoryGrid.Height;
        for (int i = 0; i < totalCells; i++) {
            cells[i].SetIndex(i);
        }

        BuildFromGrid();
    }

    public void BuildFromGrid() {
        ClearUI();

        foreach (var item in inventoryGrid.GetItems()) {
            CreateInventoryItemUI(item);
        }
    }

    public void CreateInventoryItemUI(InventoryItem item) {
        InventoryItemUI itemUI = Instantiate(itemPrefab, itemsParent);
        itemUI.Init(item, this);
        itemToUI[item] = itemUI;
        SnapToGrid(itemUI, new Vector2Int(itemUI.Item.x, itemUI.Item.y));
    }

    public void SnapToGrid(InventoryItemUI itemUI, Vector2Int pos) {
        RectTransform cellRect = GetCellRect(pos);
        if (cellRect == null) return;

        itemUI.Rect.SetParent(itemsParent);
        itemUI.Rect.sizeDelta = new Vector2(
            itemUI.Item.Width * defaultCellSize.x,
            itemUI.Item.Height * defaultCellSize.y
        );
        itemUI.Rect.anchoredPosition = cellRect.anchoredPosition + new Vector2(-cellRect.sizeDelta.x * 0.5f, cellRect.sizeDelta.y * 0.5f);
    }

    public void PreviewPlacement(InventoryItem item, Vector2Int pos) {
        ClearPreview();

        bool canPlace = inventoryGrid.CanPlaceItem(item, pos.x, pos.y);
        Color color = canPlace ? Color.green : Color.red;

        for (int x = 0; x < item.Width; x++) {
            for (int y = 0; y < item.Height; y++) {

                int gx = pos.x + x;
                int gy = pos.y + y;

                if (gx < 0 || gy < 0 || gx >= inventoryGrid.Width || gy >= inventoryGrid.Height) continue;

                int index = gy * inventoryGrid.Width + gx;

                cells[index].SetHighlight(color);
            }
        }
    }

    private Vector2Int GetGridPosFromItem(InventoryItemUI itemUI) {
        Vector2 screenPos = new Vector2(itemUI.Rect.position.x, itemUI.Rect.position.y) + new Vector2(defaultCellSize.x * 0.5f, -defaultCellSize.y * 0.5f);

        for (int y = 0; y < inventoryGrid.Height; y++) {
            for (int x = 0; x < inventoryGrid.Width; x++) {
                int index = y * inventoryGrid.Width + x;
                RectTransform cell = (RectTransform)cells[index].transform;

                if (RectTransformUtility.RectangleContainsScreenPoint(cell, screenPos, null)) {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    public RectTransform GetCellRect(Vector2Int pos) {
        if (pos.x < 0 || pos.y < 0) return null;

        if (pos.x >= inventoryGrid.Width || pos.y >= inventoryGrid.Height) return null;

        int index = pos.y * inventoryGrid.Width + pos.x;

        if (index < 0 || index >= cells.Length) return null;

        return (RectTransform)cells[index].transform;
    }

    public bool IsInsideGrid(Vector2Int pos) {
        return pos.x >= 0 &&
               pos.y >= 0 &&
               pos.x < inventoryGrid.Width &&
               pos.y < inventoryGrid.Height;
    }

    public void ClearUI() {
        foreach (Transform child in itemsParent) Destroy(child.gameObject);
        itemToUI.Clear();
    }

    public void ClearPreview() {
        foreach (var cell in cells) cell.ClearHighlight();
    }

    public Vector2 DefaultCellSize => defaultCellSize;

    public void OnBeginDrag(InventoryItemUI itemUI, PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIUnequipItem);
    }

    public void OnDrag(InventoryItemUI itemUI, PointerEventData eventData) {
        itemUI.Rect.position = eventData.position + new Vector2(-itemUI.Rect.sizeDelta.x * 0.5f, itemUI.Rect.sizeDelta.y * 0.5f);
    }

    public bool CanReceiveItem(InventoryItemUI itemUI, PointerEventData eventData) {
        Vector2Int gridPos = GetGridPosFromItem(itemUI);

        if (!IsInsideGrid(gridPos)) return false;

        return inventoryGrid.CanPlaceItem(itemUI.Item, gridPos.x, gridPos.y);
    }

    public void ReceiveItem(InventoryItemUI itemUI, PointerEventData eventData) {
        Vector2Int gridPos = GetGridPosFromItem(itemUI);

        isReceivingItem = true;
        inventoryGrid.RemoveItem(itemUI.Item);
        inventoryGrid.PlaceItem(itemUI.Item, gridPos.x, gridPos.y);
        isReceivingItem = false;

        itemToUI[itemUI.Item] = itemUI;
        SnapToGrid(itemUI, gridPos);

        AudioManager.Instance.PlaySFX(SFX.UIEquipItem);
    }

    public void CancelDrag(InventoryItemUI itemUI, PointerEventData eventData) {
        SnapToGrid(itemUI, new Vector2Int(itemUI.Item.x, itemUI.Item.y));
    }

    public void DetachItem(InventoryItemUI itemUI) {
        inventoryGrid.RemoveItem(itemUI.Item);
        itemToUI.Remove(itemUI.Item);
    }

    public void RemoveItem(InventoryItemUI itemUI) {
        if (itemToUI.TryGetValue(itemUI.Item, out var ui)) {
            DetachItem(itemUI);
            Destroy(ui.gameObject);
        }
    }

    public void ShowVisualState(InventoryItemUI itemUI, PointerEventData eventData) {
        Vector2Int gridPos = GetGridPosFromItem(itemUI);
        if (gridPos.x >= 0 && gridPos.y >= 0) {
            PreviewPlacement(itemUI.Item, gridPos);
        }
    }

    public void ClearVisualState(InventoryItemUI itemUI) {
        ClearPreview();
    }

    void OnEnable() {
        InventoryGrid.OnItemAdded += HandleItemAdded;
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void OnDisable() {
        InventoryGrid.OnItemAdded -= HandleItemAdded;
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    void HandleItemAdded(InventoryItem item) {
        if (isReceivingItem) return;

        CreateInventoryItemUI(item);
    }

    private void HandlePlayerReady(Player p) {
        if (inventoryGrid != null) return;
        inventoryGrid = p.InventoryGrid;
        Initialize();
    }
}
