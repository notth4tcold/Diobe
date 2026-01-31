using UnityEngine;

public class InventoryGrid : MonoBehaviour {
    private int width = 11;
    private int height = 5;

    private InventoryCell[,] grid;

    void Awake() {
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
                if (grid[startX + x, startY + y].item == item) return true;
                if (grid[startX + x, startY + y].occupied) return false;
            }
        }

        return true;
    }

    public bool PlaceItem(InventoryItem item, int startX, int startY) {
        if (!CanPlaceItem(item, startX, startY))
            return false;

        item.x = startX;
        item.y = startY;

        for (int x = 0; x < item.Width; x++)
            for (int y = 0; y < item.Height; y++) {
                grid[startX + x, startY + y].occupied = true;
                grid[startX + x, startY + y].item = item;
            }

        return true;
    }

    public void RemoveItem(InventoryItem item) {
        for (int x = 0; x < item.Width; x++)
            for (int y = 0; y < item.Height; y++) {
                grid[item.x + x, item.y + y].occupied = false;
                grid[item.x + x, item.y + y].item = null;
            }
    }

    public int Width => width;
    public int Height => height;
}
