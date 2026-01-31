[System.Serializable]
public class InventoryItem {
    public ItemData data;
    public int x;
    public int y;

    public InventoryItem(ItemData data, int x, int y) {
        this.data = data;
        this.x = x;
        this.y = y;
    }

    public int Width => data.width;
    public int Height => data.height;
}
