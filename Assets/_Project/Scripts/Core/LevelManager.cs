using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject mapPrefab;
    [SerializeField] GameObject itemPickupPrefab;

    [Header("ItemData")]
    [SerializeField] private ItemData sword;

    GameObject playerInstance;
    GameObject mapInstance;

    private GameSaveData gameSave;
    private Player player;

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() {
        EnsureManager<GameManager>();
        EnsureManager<AudioManager>();
        EnsureManager<ItemDatabase>();

        GameManager.Instance.NewCharacter("teste", 1);
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

        gameSave = GameManager.Instance.gameSaveData;

        SpawnMap(gameSave.hasMapPosition ? gameSave.mapPosition : defaultMapPos);
        SpawnPlayer(gameSave.hasPlayerPosition ? gameSave.playerPosition : defaultPlayerPos, gameSave);
        SpawnItem(Vector2.zero, sword);
        SpawnItem(Vector2.zero, sword);
    }

    void Start() {
        CameraManager.Instance.SetTarget(GetPlayer().transform);

        if (gameSave.isNewPlayer) {
            GameManager.Instance.addInitialItems();
            player.ResetHeathAndMana();
            gameSave.isNewPlayer = false;
        }
    }


    void SpawnPlayer(Vector2 position, GameSaveData gameSave) {
        playerInstance = Instantiate(playerPrefab, new Vector2(position.x, position.y), Quaternion.identity);

        player = playerInstance.GetComponent<Player>();
        player.id = gameSave.characterSaveData.id;
        player.playerName = gameSave.characterSaveData.playerName;
        player.characterClass = gameSave.characterSaveData.characterClass;
        player.money = gameSave.characterSaveData.money;
        player.level = gameSave.characterSaveData.level;
        player.exp = gameSave.characterSaveData.exp;
        player.stats = gameSave.characterSaveData.stats;
        player.resources = gameSave.characterSaveData.resources;
        player.combat = gameSave.characterSaveData.combat;

        player.InitializeResourcesAndCombat();
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

    public Player GetPlayer() => player;

    public Vector2 GetMapTransform() => mapInstance.transform.position;
}
