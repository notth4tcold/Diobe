using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour, IItemContainerUI {
    [Header("References")]
    [SerializeField] private Transform itemsParent;
    [SerializeField] private ItemUI itemPrefab;
    [SerializeField] private InventoryCellUI[] cells;

    private Dictionary<Item, ItemUI> itemToUI = new();
    private bool isReceivingItem;
    private Vector2 defaultCellSize;
    private Player player;

    void Awake() {
        defaultCellSize = ((RectTransform)cells[0].transform).sizeDelta;
    }

    void Initialize() {
        int totalCells = player.InventoryGrid.Width * player.InventoryGrid.Height;
        for (int i = 0; i < totalCells; i++) {
            cells[i].SetIndex(i);
        }

        BuildFromGrid();
    }

    public void BuildFromGrid() {
        ClearUI();

        foreach (var item in player.InventoryGrid.GetItems()) {
            CreateInventoryItemUI(item);
        }
    }

    public void CreateInventoryItemUI(Item item) {
        ItemUI itemUI = Instantiate(itemPrefab, itemsParent);
        itemUI.Init(item, this);
        itemToUI[item] = itemUI;
        SnapToGrid(itemUI, new Vector2Int(itemUI.Item.x, itemUI.Item.y));
    }

    public void SnapToGrid(ItemUI itemUI, Vector2Int pos) {
        RectTransform cellRect = GetCellRect(pos);
        if (cellRect == null) return;

        itemUI.Rect.SetParent(itemsParent);
        itemUI.Rect.sizeDelta = new Vector2(
            itemUI.Item.Width * defaultCellSize.x,
            itemUI.Item.Height * defaultCellSize.y
        );
        itemUI.Rect.anchoredPosition = cellRect.anchoredPosition + new Vector2(-cellRect.sizeDelta.x * 0.5f, cellRect.sizeDelta.y * 0.5f);
    }

    public void PreviewPlacement(Item item, Vector2Int pos) {
        ClearPreview();

        bool canPlace = player.InventoryGrid.CanPlaceItem(item, pos.x, pos.y);
        Color color = canPlace ? Color.green : Color.red;

        for (int x = 0; x < item.Width; x++) {
            for (int y = 0; y < item.Height; y++) {

                int gx = pos.x + x;
                int gy = pos.y + y;

                if (gx < 0 || gy < 0 || gx >= player.InventoryGrid.Width || gy >= player.InventoryGrid.Height) continue;

                int index = gy * player.InventoryGrid.Width + gx;

                cells[index].SetHighlight(color);
            }
        }
    }

    private Vector2Int GetGridPosFromItem(ItemUI itemUI) {
        Vector2 screenPos = new Vector2(itemUI.Rect.position.x, itemUI.Rect.position.y) + new Vector2(defaultCellSize.x * 0.5f, -defaultCellSize.y * 0.5f);

        for (int y = 0; y < player.InventoryGrid.Height; y++) {
            for (int x = 0; x < player.InventoryGrid.Width; x++) {
                int index = y * player.InventoryGrid.Width + x;
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

        if (pos.x >= player.InventoryGrid.Width || pos.y >= player.InventoryGrid.Height) return null;

        int index = pos.y * player.InventoryGrid.Width + pos.x;

        if (index < 0 || index >= cells.Length) return null;

        return (RectTransform)cells[index].transform;
    }

    public bool IsInsideGrid(Vector2Int pos) {
        return pos.x >= 0 &&
               pos.y >= 0 &&
               pos.x < player.InventoryGrid.Width &&
               pos.y < player.InventoryGrid.Height;
    }

    public void ClearUI() {
        foreach (Transform child in itemsParent) Destroy(child.gameObject);
        itemToUI.Clear();
    }

    public void ClearPreview() {
        foreach (var cell in cells) cell.ClearHighlight();
    }

    public Vector2 DefaultCellSize => defaultCellSize;

    public void OnBeginDrag(ItemUI itemUI, PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIUnequipItem);
    }

    public void OnDrag(ItemUI itemUI, PointerEventData eventData) {
        itemUI.Rect.position = eventData.position + new Vector2(-itemUI.Rect.sizeDelta.x * 0.5f, itemUI.Rect.sizeDelta.y * 0.5f);
    }

    public bool CanReceiveItem(ItemUI itemUI, PointerEventData eventData) {
        Vector2Int gridPos = GetGridPosFromItem(itemUI);

        if (!IsInsideGrid(gridPos)) return false;

        return player.InventoryGrid.CanPlaceItem(itemUI.Item, gridPos.x, gridPos.y);
    }

    public void ReceiveItem(ItemUI itemUI, PointerEventData eventData) {
        Vector2Int gridPos = GetGridPosFromItem(itemUI);

        isReceivingItem = true;
        player.InventoryGrid.RemoveItem(itemUI.Item);
        player.InventoryGrid.PlaceItem(itemUI.Item, gridPos.x, gridPos.y);
        isReceivingItem = false;

        itemToUI[itemUI.Item] = itemUI;
        SnapToGrid(itemUI, gridPos);

        AudioManager.Instance.PlaySFX(SFX.UIEquipItem);
    }

    public void CancelDrag(ItemUI itemUI, PointerEventData eventData) {
        SnapToGrid(itemUI, new Vector2Int(itemUI.Item.x, itemUI.Item.y));
    }

    public void DetachItem(ItemUI itemUI) {
        player.InventoryGrid.RemoveItem(itemUI.Item);
        itemToUI.Remove(itemUI.Item);
    }

    public void RemoveItem(ItemUI itemUI) {
        if (itemToUI.TryGetValue(itemUI.Item, out var ui)) {
            DetachItem(itemUI);
            Destroy(ui.gameObject);
        }
    }

    public void ShowVisualState(ItemUI itemUI, PointerEventData eventData) {
        Vector2Int gridPos = GetGridPosFromItem(itemUI);
        if (gridPos.x >= 0 && gridPos.y >= 0) {
            PreviewPlacement(itemUI.Item, gridPos);
        }
    }

    public void ClearVisualState(ItemUI itemUI) {
        ClearPreview();
    }

    void OnEnable() {
        InventoryGrids.OnItemAdded += HandleItemAdded;
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void OnDisable() {
        InventoryGrids.OnItemAdded -= HandleItemAdded;
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    void HandleItemAdded(Item item) {
        if (isReceivingItem) return;

        CreateInventoryItemUI(item);
    }

    private void HandlePlayerReady(Player p) {
        if (player != null) return;
        player = p;
        Initialize();
    }
}
