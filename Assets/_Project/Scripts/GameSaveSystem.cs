using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveSystem {
    static string BasePath => Application.persistentDataPath + "/Saves";

    static string IndexPath => BasePath + "/game.json";

    static string GamePath(string id) => BasePath + $"/game_{id}.json";


    public static void SaveGame(GameSaveData gameSaveData) {
        Directory.CreateDirectory(BasePath);

        gameSaveData.characterSaveData.lastSave = System.DateTime.Now;

        File.WriteAllText(GamePath(gameSaveData.characterSaveData.id), JsonUtility.ToJson(gameSaveData, true));

        var index = LoadIndex() ?? new GameSaveIndex();

        if (!index.gameSaveIds.Contains(gameSaveData.characterSaveData.id)) index.gameSaveIds.Add(gameSaveData.characterSaveData.id);

        File.WriteAllText(IndexPath, JsonUtility.ToJson(index, true));

        Debug.Log("Game saved - Player " + gameSaveData.characterSaveData.playerName + " as " + gameSaveData.characterSaveData.characterClass);
    }

    static GameSaveData LoadGame(string id) {
        if (!File.Exists(GamePath(id))) return null;

        return JsonUtility.FromJson<GameSaveData>(File.ReadAllText(GamePath(id)));
    }

    public static List<GameSaveData> LoadAllGameSaves() {
        var index = LoadIndex();
        var list = new List<GameSaveData>();

        if (index == null) return list;

        foreach (var id in index.gameSaveIds) {
            var data = LoadGame(id);
            if (data != null) list.Add(data);
        }

        return list;
    }

    static GameSaveIndex LoadIndex() {
        if (!File.Exists(IndexPath)) return null;

        return JsonUtility.FromJson<GameSaveIndex>(
            File.ReadAllText(IndexPath)
        );
    }
}

public class GameSaveIndex {
    public List<string> gameSaveIds = new();
}
