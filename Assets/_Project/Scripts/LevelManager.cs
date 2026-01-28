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
        if (GameManager.Instance.HasLoadedGame()) SpawnFromSave();
        else SpawnNewGame();
    }

    void SpawnNewGame() {
        SpawnMap(new Vector2(0, -3.38f));
        SpawnPlayer(Vector2.zero);
    }

    void SpawnFromSave() {
        var gameSave = GameManager.Instance.gameSaveData;
        SpawnMap(gameSave.mapPosition);
        SpawnPlayer(gameSave.playerPosition);
        Debug.Log("Game loaded sucessfully");
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
