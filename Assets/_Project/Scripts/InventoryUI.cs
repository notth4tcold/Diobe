using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform itemsParent;
    [SerializeField] private InventoryItemUI itemPrefab;
    [SerializeField] private InventoryCellUI[] cells;

    private Dictionary<InventoryItem, InventoryItemUI> itemToUI = new();
    private InventoryGrid inventoryGrid;

    void Awake() {
        inventoryGrid = GameManager.Instance.InventoryGrid;
    }

    void Start() {
        ConfigGridIndex();
        BuildFromGrid();
    }

    void ConfigGridIndex() {
        int totalCells = inventoryGrid.Width * inventoryGrid.Height;

        for (int i = 0; i < totalCells; i++) {
            cells[i].SetIndex(i);
        }
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
    }

    public Vector2Int ScreenToGrid(Vector2 screenPos) {
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

    public Vector2 GridToScreen(int x, int y) {
        int index = y * inventoryGrid.Width + x;
        if (index < 0 || index >= cells.Length) return Vector2.zero;

        RectTransform cellRect = (RectTransform)cells[index].transform;

        return cellRect.anchoredPosition + new Vector2(-CellSize * 0.5f, CellSize * 0.5f);
    }

    public RectTransform GetCellRect(Vector2Int pos) {
        if (pos.x < 0 || pos.y < 0) return null;

        if (pos.x >= inventoryGrid.Width || pos.y >= inventoryGrid.Height) return null;

        int index = pos.y * inventoryGrid.Width + pos.x;

        if (index < 0 || index >= cells.Length) return null;

        return (RectTransform)cells[index].transform;
    }

    public void TryPlaceItem(InventoryItem item, InventoryItemUI itemUI, Vector2Int gridPos) {
        int oldX = item.x;
        int oldY = item.y;

        inventoryGrid.RemoveItem(item);

        if (!inventoryGrid.CanPlaceItem(item, gridPos.x, gridPos.y)) {
            inventoryGrid.PlaceItem(item, oldX, oldY);
            itemUI.SnapToGrid(new Vector2Int(oldX, oldY));
            return;
        }

        inventoryGrid.PlaceItem(item, gridPos.x, gridPos.y);
        itemUI.SnapToGrid(gridPos);
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

    public void ClearPreview() {
        foreach (var cell in cells)
            cell.ClearHighlight();
    }

    public void ClearUI() {
        foreach (Transform child in itemsParent)
            Destroy(child.gameObject);

        itemToUI.Clear();
    }

    void OnEnable() {
        GameManager.OnItemAdded += HandleItemAdded;
        GameManager.OnItemRemoved += HandleItemRemoved;
    }

    void OnDisable() {
        GameManager.OnItemAdded -= HandleItemAdded;
        GameManager.OnItemRemoved -= HandleItemRemoved;
    }

    void HandleItemAdded(InventoryItem item) {
        CreateInventoryItemUI(item);
    }

    void HandleItemRemoved(InventoryItem item) {
        if (itemToUI.TryGetValue(item, out var ui)) {
            Destroy(ui.gameObject);
            itemToUI.Remove(item);
        }
    }

    public float CellSize => ((RectTransform)cells[0].transform).sizeDelta.x;
}
