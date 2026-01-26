using UnityEngine;

public class LevelManager : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject mapPrefab;

    void Start() {
        SpawnMap();
        SpawnPlayer();
    }

    void SpawnMap() {
        Instantiate(mapPrefab, new Vector2(0, -3.38f), Quaternion.identity);
    }

    void SpawnPlayer() {
        Instantiate(playerPrefab, Vector2.zero, Quaternion.identity);
    }
}
