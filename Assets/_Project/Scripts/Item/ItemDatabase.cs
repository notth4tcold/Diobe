using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject {

    [SerializeField] private ItemData[] items;

    private Dictionary<string, ItemData> lookup;

    void Init() {
        if (lookup != null) return;

        lookup = new Dictionary<string, ItemData>();

        foreach (var item in items) {
            if (string.IsNullOrEmpty(item.id)) {
                Debug.LogError($"ItemData sem ID: {item.name}");
                continue;
            }

            if (lookup.ContainsKey(item.id)) {
                Debug.LogError($"Item ID duplicado: {item.id}");
                continue;
            }

            lookup[item.id] = item;
        }
    }

    public ItemData Get(string id) {
        Init();

        if (!lookup.TryGetValue(id, out var item)) {
            Debug.LogError($"ItemData não encontrado: {id}");
            return null;
        }

        return item;
    }
}
