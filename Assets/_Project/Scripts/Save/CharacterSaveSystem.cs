
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CharacterSaveSystem {
    static string BasePath => Application.persistentDataPath + "/Saves";

    static string IndexPath => BasePath + "/characters.json";

    static string CharPath(string id) => BasePath + $"/char_{id}.json";

    public static void SaveCharacter(CharacterSaveData data) {
        Directory.CreateDirectory(BasePath);

        data.lastSave = System.DateTime.Now;

        File.WriteAllText(CharPath(data.id), JsonUtility.ToJson(data, true));

        var index = LoadIndex() ?? new CharacterIndex();

        if (!index.characterIds.Contains(data.id)) index.characterIds.Add(data.id);

        File.WriteAllText(IndexPath, JsonUtility.ToJson(index, true));

        Debug.Log("Character saved - Player " + data.playerName + " as " + data.characterClass);
    }

    static CharacterSaveData LoadCharacter(string id) {
        if (!File.Exists(CharPath(id))) return null;

        return JsonUtility.FromJson<CharacterSaveData>(File.ReadAllText(CharPath(id)));
    }

    public static List<CharacterSaveData> LoadAllCharacters() {
        var index = LoadIndex();
        var list = new List<CharacterSaveData>();

        if (index == null) return list;

        foreach (var id in index.characterIds) {
            var data = LoadCharacter(id);
            if (data != null) list.Add(data);
        }

        return list;
    }

    static CharacterIndex LoadIndex() {
        if (!File.Exists(IndexPath)) return null;

        return JsonUtility.FromJson<CharacterIndex>(
            File.ReadAllText(IndexPath)
        );
    }
}

public class CharacterIndex {
    public List<string> characterIds = new();
}
