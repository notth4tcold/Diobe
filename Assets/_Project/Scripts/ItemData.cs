using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject {
    public string id;
    public string itemName;
    public Sprite icon;

    public int width = 1;
    public int height = 1;
}
