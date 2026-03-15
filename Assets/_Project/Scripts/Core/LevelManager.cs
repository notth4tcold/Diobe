using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject mapPrefab;
    [SerializeField] GameObject itemPickupPrefab;

    // TODO remove test
    [SerializeField] private ItemData sword;

    GameObject playerInstance;
    GameObject mapInstance;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() {
        EnsureManager<GameManager>();
        EnsureManager<AudioManager>();

        GameManager.Instance.LoadGameSaveDataForNewCharacter("teste", 1);
    }

    static void EnsureManager<T>() where T : Component {
        if (FindFirstObjectByType<T>() != null) return;

        var prefab = GetPrefabFromEditor<T>();
        if (prefab != null) Instantiate(prefab);
    }

    static GameObject GetPrefabFromEditor<T>() where T : Component {
        string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:Prefab {typeof(T).Name}");
        if (guids.Length == 0) return null;

        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
        return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }
#endif

    void Awake() {
        Instance = this;

        Vector2 defaultMapPos = new Vector2(0, -3.38f);
        Vector2 defaultPlayerPos = Vector2.zero;

        var gameSave = GameManager.Instance.gameSaveData;

        SpawnMap(gameSave.hasMapPosition ? gameSave.mapPosition : defaultMapPos);
        SpawnPlayer(gameSave.hasPlayerPosition ? gameSave.playerPosition : defaultPlayerPos);

        // TODO remove test
        Item item = new Item(sword, 0, 0);
        item.GenerateModifiers(1, LootQuality.Boss);
        //item.modifiers.Add(new ItemModifier(StatType.MaxHealth, 1000));
        SpawnItem(Vector2.zero, item);

        Item item2 = new Item(sword, 0, 0);
        item2.GenerateModifiers(1, LootQuality.Boss);
        //item2.modifiers.Add(new ItemModifier(StatType.AttackSpeed, 1000));
        SpawnItem(Vector2.zero, item2);

        Item item3 = new Item(sword, 0, 0);
        item3.GenerateModifiers(1, LootQuality.Boss);
        //item3.modifiers.Add(new ItemModifier(StatType.MoveSpeed, 1));
        SpawnItem(Vector2.zero, item3);

        Item item4 = new Item(sword, 0, 0);
        item4.GenerateModifiers(1, LootQuality.Boss);
        //item4.modifiers.Add(new ItemModifier(StatType.MaxHealth, 1000));
        //item4.modifiers.Add(new ItemModifier(StatType.HealthRegen, 10));
        SpawnItem(Vector2.zero, item4);
    }

    void SpawnPlayer(Vector2 position) {
        playerInstance = Instantiate(playerPrefab, new Vector2(position.x, position.y), Quaternion.identity);
    }

    void SpawnMap(Vector2 position) {
        mapInstance = Instantiate(mapPrefab, new Vector2(position.x, position.y), Quaternion.identity);
    }

    public void SpawnItem(Vector2 position, Item item) {
        GameObject itemInstance = Instantiate(itemPickupPrefab, position, Quaternion.identity);
        ItemPickup pickup = itemInstance.GetComponent<ItemPickup>();
        pickup.Initialize(item);

        Rigidbody2D rb = itemInstance.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), 4f), ForceMode2D.Impulse);
    }

    public Vector2 GetPlayerTransform() => playerInstance.transform.position;
    public Vector2 GetMapTransform() => mapInstance.transform.position;
}
