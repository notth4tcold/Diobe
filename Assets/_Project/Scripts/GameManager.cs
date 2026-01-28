using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public CharacterSaveData characterSaveData;

    public GameSaveData gameSaveData;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<CharacterSaveData> GetAllCharacters() {
        return CharacterSaveSystem.LoadAllCharacters();
    }

    public List<GameSaveData> GetAllGameSaves() {
        return GameSaveSystem.LoadAllGameSaves();
    }

    public void NewCharacterSaveData(string name, int classs) {
        characterSaveData = new CharacterSaveData {
            id = System.Guid.NewGuid().ToString(),
            playerName = name,
            characterClass = (CharacterClass)classs,
        };
    }

    public void FillCharacterSaveData(CharacterSaveData character) {
        characterSaveData = character;
    }

    public void FillGameSaveData(GameSaveData gameSave) {
        gameSaveData = gameSave;
    }

    public void FillGameSaveData() {
        gameSaveData.characterSaveData = characterSaveData;
        gameSaveData.playerPosition = LevelManager.Instance.GetPlayerTransform();
        gameSaveData.mapPosition = LevelManager.Instance.GetMapTransform();
    }

    public bool HasLoadedGame() {
        return gameSaveData != null
            && gameSaveData.characterSaveData != null
            && gameSaveData.characterSaveData.id != null
            && gameSaveData.characterSaveData.id != "";
    }

    public void ResetGameState() {
        characterSaveData = null;
        gameSaveData = null;
    }
}
