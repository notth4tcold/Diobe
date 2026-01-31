using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    [Header("References")]
    [SerializeField] private InventoryGrid inventoryGrid;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private InventoryCellUI cellPrefab;
    [SerializeField] private InventoryItemUI itemPrefab;

    private float cellSize;
    private InventoryCellUI[] cells;

    [SerializeField]
    private ItemData[] itemsToSpawn;

    void Start() {
        CreateGrid();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            (RectTransform)gridParent
        );

        StartCoroutine(SpawnAfterLayout());
    }

    void CreateGrid() {
        int totalCells = inventoryGrid.Width * inventoryGrid.Height;
        cells = new InventoryCellUI[totalCells];

        InventoryCellUI temp = Instantiate(cellPrefab, gridParent);
        cellSize = ((RectTransform)temp.transform).sizeDelta.x;
        Destroy(temp.gameObject);

        for (int i = 0; i < totalCells; i++) {
            InventoryCellUI cell = Instantiate(cellPrefab, gridParent);
            cell.SetIndex(i);
            cells[i] = cell;
        }
    }

    IEnumerator SpawnAfterLayout() {
        yield return null;

        SpawnItems();
    }

    public void SpawnItems() {
        for (int i = 0; i < itemsToSpawn.Length; i++) {
            InventoryItem item = new InventoryItem(itemsToSpawn[i], 0, 0);
            Vector2Int pos;

            if (!inventoryGrid.FindEmptyPlace(item, out pos)) return;
            inventoryGrid.PlaceItem(item, pos.x, pos.y);

            InventoryItemUI itemUI = Instantiate(itemPrefab, itemsParent);
            itemUI.Init(item, this);
        }
    }

    public Vector2 GridToScreen(int x, int y) {
        int index = y * inventoryGrid.Width + x;
        if (index < 0 || index >= cells.Length) return Vector2.zero;

        RectTransform cellRect = (RectTransform)cells[index].transform;

        return cellRect.anchoredPosition + new Vector2(-CellSize * 0.5f, CellSize * 0.5f);
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

        Debug.Log(canPlace);

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

    public RectTransform GetCellRect(Vector2Int pos) {
        if (pos.x < 0 || pos.y < 0) return null;

        if (pos.x >= inventoryGrid.Width || pos.y >= inventoryGrid.Height) return null;

        int index = pos.y * inventoryGrid.Width + pos.x;

        if (index < 0 || index >= cells.Length) return null;

        return (RectTransform)cells[index].transform;
    }

    public float CellSize => cellSize;
}
