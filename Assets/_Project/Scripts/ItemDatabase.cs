using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
    public static ItemDatabase Instance { get; private set; }

    [SerializeField] private ItemData[] items;

    private Dictionary<string, ItemData> lookup;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

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
        if (!lookup.TryGetValue(id, out var item)) {
            Debug.LogError($"ItemData não encontrado: {id}");
            return null;
        }

        return item;
    }
}
