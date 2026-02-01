using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject mapPrefab;

    GameObject playerInstance;
    GameObject mapInstance;

    void Awake() {
        Instance = this;
    }

    void Start() {
        Vector2 defaultMapPos = new Vector2(0, -3.38f);
        Vector2 defaultPlayerPos = Vector2.zero;

        var gameSave = GameManager.Instance.gameSaveData;

        SpawnMap(gameSave.hasMapPosition ? gameSave.mapPosition : defaultMapPos);
        SpawnPlayer(gameSave.hasPlayerPosition ? gameSave.playerPosition : defaultPlayerPos);
    }

    void SpawnPlayer(Vector2 position) {
        playerInstance = Instantiate(playerPrefab, new Vector2(position.x, position.y), Quaternion.identity);
    }

    void SpawnMap(Vector2 position) {
        mapInstance = Instantiate(mapPrefab, new Vector2(position.x, position.y), Quaternion.identity);
    }

    public Vector2 GetPlayerTransform() => playerInstance.transform.position;

    public Vector2 GetMapTransform() => mapInstance.transform.position;
}
