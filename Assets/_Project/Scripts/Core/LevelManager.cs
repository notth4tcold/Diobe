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

    void Awake() {
        Instance = this;

        Vector2 defaultMapPos = new Vector2(0, -3.38f);
        Vector2 defaultPlayerPos = Vector2.zero;

        var gameSave = GameManager.Instance.gameSaveData;

        SpawnMap(gameSave.hasMapPosition ? gameSave.mapPosition : defaultMapPos);
        SpawnPlayer(gameSave.hasPlayerPosition ? gameSave.playerPosition : defaultPlayerPos, gameSave);
        SpawnItem(Vector2.zero, sword);
    }


    void Start() {
        CameraManager.Instance.SetTarget(GetPlayer().transform);
    }


    void SpawnPlayer(Vector2 position, GameSaveData gameSave) {
        playerInstance = Instantiate(playerPrefab, new Vector2(position.x, position.y), Quaternion.identity);

        var player = playerInstance.GetComponent<Player>();
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

        if (gameSave.isNewPlayer) {
            player.ResetHeathAndMana();
            gameSave.isNewPlayer = false;
        }
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

    public Player GetPlayer() => playerInstance.GetComponent<Player>();

    public Vector2 GetMapTransform() => mapInstance.transform.position;
}
