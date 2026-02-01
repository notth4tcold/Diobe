using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public InventoryGrid InventoryGrid { get; private set; }

    [SerializeField]
    private ItemData[] startingItems;

    public static event Action<InventoryItem> OnItemAdded;
    public static event Action<InventoryItem> OnItemRemoved;

    public GameSaveData gameSaveData;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InventoryGrid = GetComponent<InventoryGrid>();
    }

    public List<CharacterSaveData> GetAllCharacters() {
        return CharacterSaveSystem.LoadAllCharacters();
    }

    public List<GameSaveData> GetAllGameSaves() {
        return GameSaveSystem.LoadAllGameSaves();
    }

    public void NewCharacter(string name, int classID) {
        spawnInitialItems();

        var characterSaveData = new CharacterSaveData {
            id = System.Guid.NewGuid().ToString(),
            playerName = name,
            level = 0,
            exp = 0,
            lastSave = DateTime.Now,
            characterClass = (CharacterClass)classID,
            items = InventoryGrid.BuildSaveData()
        };

        gameSaveData = new GameSaveData {
            characterSaveData = characterSaveData,
            playerPosition = Vector2.zero,
            hasPlayerPosition = false,
            mapPosition = Vector2.zero,
            hasMapPosition = false
        };
    }

    public void ImportCharacter(CharacterSaveData character) {
        LoadItems(character.items);

        gameSaveData = new GameSaveData {
            characterSaveData = character,
            playerPosition = Vector2.zero,
            hasPlayerPosition = false,
            mapPosition = Vector2.zero,
            hasMapPosition = false
        };
    }

    public void LoadGame(GameSaveData gameSave) {
        LoadItems(gameSave.characterSaveData.items);

        gameSaveData = gameSave;
    }

    public void SaveCharacter() {
        gameSaveData.characterSaveData.items = InventoryGrid.BuildSaveData();
        CharacterSaveSystem.SaveCharacter(gameSaveData.characterSaveData);
    }

    public void SaveGame() {
        gameSaveData.characterSaveData.items = InventoryGrid.BuildSaveData();
        gameSaveData.playerPosition = LevelManager.Instance.GetPlayerTransform();
        gameSaveData.hasPlayerPosition = true;
        gameSaveData.mapPosition = LevelManager.Instance.GetMapTransform();
        gameSaveData.hasMapPosition = true;
        GameSaveSystem.SaveGame(gameSaveData);
    }

    public void spawnInitialItems() {
        InventoryGrid.ResetGrid();

        foreach (var data in startingItems) SpawnItem(data);
    }

    public void LoadItems(List<InventoryItemSaveData> items) {
        InventoryGrid.ResetGrid();

        foreach (var saveItem in items) {
            ItemData data = ItemDatabase.Instance.Get(saveItem.itemId);

            InventoryItem item = new InventoryItem(data, saveItem.x, saveItem.y);

            InventoryGrid.PlaceItem(item, item.x, item.y);
            OnItemAdded?.Invoke(item);
        }
    }

    public void SpawnItem(ItemData data) {
        InventoryItem item = new InventoryItem(data, 0, 0);

        if (InventoryGrid.FindEmptyPlace(item, out Vector2Int pos)) {
            InventoryGrid.PlaceItem(item, pos.x, pos.y);
            OnItemAdded?.Invoke(item);
        }
    }

    public void RemoveItem(InventoryItem item) {
        InventoryGrid.RemoveItem(item);
        OnItemRemoved?.Invoke(item);
    }

    public void ResetGameState() {
        gameSaveData = null;
        InventoryGrid.ResetGrid();
    }
}
