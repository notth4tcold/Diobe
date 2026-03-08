using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour {
    private int width = 11;
    private int height = 5;

    private InventoryCell[,] grid;
    public List<InventoryItem> items = new();

    public static event Action<InventoryItem> OnItemAdded;

    public void Initialize() {
        grid = new InventoryCell[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = new InventoryCell();
    }

    public bool FindEmptyPlace(InventoryItem item, out Vector2Int pos) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (CanPlaceItem(item, x, y)) {
                    pos = new Vector2Int(x, y);
                    return true;
                }
            }
        }

        pos = Vector2Int.zero;
        return false;
    }

    public bool CanPlaceItem(InventoryItem item, int startX, int startY) {
        if (startX < 0 || startY < 0) return false;
        if (startX + item.Width > width || startY + item.Height > height) return false;

        for (int x = 0; x < item.Width; x++) {
            for (int y = 0; y < item.Height; y++) {
                var cell = grid[startX + x, startY + y];
                if (cell.occupied && cell.item != item) return false;
            }
        }

        return true;
    }

    public bool SpawnItem(InventoryItem item) {
        if (FindEmptyPlace(item, out Vector2Int pos)) return PlaceItem(item, pos.x, pos.y);
        return false;
    }

    public bool PlaceItem(InventoryItem item, int startX, int startY) {
        if (!CanPlaceItem(item, startX, startY)) return false;

        item.x = startX;
        item.y = startY;

        for (int x = 0; x < item.Width; x++) {
            for (int y = 0; y < item.Height; y++) {
                grid[startX + x, startY + y].occupied = true;
                grid[startX + x, startY + y].item = item;
            }
        }

        if (!items.Contains(item)) items.Add(item);
        OnItemAdded?.Invoke(item);

        return true;
    }

    public void RemoveItem(InventoryItem item) {
        for (int x = 0; x < item.Width; x++) {
            for (int y = 0; y < item.Height; y++) {
                grid[item.x + x, item.y + y].occupied = false;
                grid[item.x + x, item.y + y].item = null;
            }
        }

        items.Remove(item);
    }

    public void ResetGrid() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                grid[x, y].occupied = false;
                grid[x, y].item = null;
            }
        }

        items.Clear();
    }

    public List<InventoryItemSaveData> BuildSaveData() {
        List<InventoryItemSaveData> itemsData = new();

        foreach (var item in items) {
            itemsData.Add(new InventoryItemSaveData {
                itemId = item.data.id,
                x = item.x,
                y = item.y,
                itemLevel = item.itemLevel,
                modifiers = item.modifiers
            });
        }

        return itemsData;
    }

    public List<InventoryItem> GetItems() {
        return items;
    }

    public int Width => width;
    public int Height => height;
}
