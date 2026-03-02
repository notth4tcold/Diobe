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
        EnsureManager<ItemDatabase>();

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
        SpawnItem(Vector2.zero, sword);
        SpawnItem(Vector2.zero, sword);
    }

    void SpawnPlayer(Vector2 position) {
        playerInstance = Instantiate(playerPrefab, new Vector2(position.x, position.y), Quaternion.identity);
    }

    void SpawnMap(Vector2 position) {
        mapInstance = Instantiate(mapPrefab, new Vector2(position.x, position.y), Quaternion.identity);
    }

    public void SpawnItem(Vector2 position, ItemData item) {
        GameObject itemInstance = Instantiate(itemPickupPrefab, position, Quaternion.identity);

        ItemPickup pickup = itemInstance.GetComponent<ItemPickup>();
        pickup.Initialize(item);

        Rigidbody2D rb = itemInstance.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), 4f), ForceMode2D.Impulse);
    }

    public Vector2 GetPlayerTransform() => playerInstance.transform.position;
    public Vector2 GetMapTransform() => mapInstance.transform.position;
}
