using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public InventoryGrid InventoryGrid { get; private set; }

    [SerializeField] private ItemData[] startingItems;

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
        var characterSaveData = new CharacterSaveData {
            id = System.Guid.NewGuid().ToString(),
            playerName = name,
            characterClass = (CharacterClass)classID,
            money = 0,
            level = 0,
            exp = 0,
            stats = GetDefaultStatsByClass((CharacterClass)classID),
            resources = new(),
            combat = new(),
            lastSave = DateTime.Now,
            items = InventoryGrid.BuildSaveData()
        };

        gameSaveData = new GameSaveData {
            characterSaveData = characterSaveData,
            playerPosition = Vector2.zero,
            hasPlayerPosition = false,
            mapPosition = Vector2.zero,
            hasMapPosition = false,
            isNewPlayer = true
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
        FillCharacterSaveData();
        CharacterSaveSystem.SaveCharacter(gameSaveData.characterSaveData);
    }

    public void SaveGame() {
        FillCharacterSaveData();
        FillGameSaveData();
        GameSaveSystem.SaveGame(gameSaveData);
    }

    public void FillCharacterSaveData() {
        var player = LevelManager.Instance.GetPlayer();

        gameSaveData.characterSaveData.id = player.id;
        gameSaveData.characterSaveData.playerName = player.playerName;
        gameSaveData.characterSaveData.characterClass = player.characterClass;
        gameSaveData.characterSaveData.money = player.money;
        gameSaveData.characterSaveData.level = player.level;
        gameSaveData.characterSaveData.exp = player.exp;
        gameSaveData.characterSaveData.stats = player.stats;
        gameSaveData.characterSaveData.resources = player.resources;
        gameSaveData.characterSaveData.combat = player.combat;

        gameSaveData.characterSaveData.items = InventoryGrid.BuildSaveData();
    }

    public void FillGameSaveData() {
        gameSaveData.playerPosition = LevelManager.Instance.GetPlayerTransform();
        gameSaveData.hasPlayerPosition = true;
        gameSaveData.mapPosition = LevelManager.Instance.GetMapTransform();
        gameSaveData.hasMapPosition = true;
    }

    public void addInitialItems() {
        InventoryGrid.ResetGrid();
        foreach (var data in startingItems) PickupItem(data);
    }

    public void LoadItems(List<InventoryItemSaveData> items) {
        InventoryGrid.ResetGrid();

        foreach (var saveItem in items) {
            ItemData data = ItemDatabase.Instance.Get(saveItem.itemId);
            InventoryItem item = new InventoryItem(data, saveItem.x, saveItem.y);
            InventoryGrid.AddNewItem(item);
        }
    }

    public bool PickupItem(ItemData data) {
        InventoryItem item = new InventoryItem(data, 0, 0);

        Player player = LevelManager.Instance.GetPlayer();
        if (item.data.type == ItemType.Weapon && !player.HasWeapon) {
            return player.EquipWeaponToInventory(item);
        }

        return SpawnNewItem(item);
    }

    public bool SpawnNewItem(InventoryItem item) {
        return InventoryGrid.SpawnNewItem(item);
    }

    public void DropItem(InventoryItem item) {
        Vector2 playerPos = LevelManager.Instance.GetPlayerTransform();
        LevelManager.Instance.SpawnItem(playerPos, item.data);
    }

    public void ResetGameState() {
        gameSaveData = null;
        InventoryGrid.ResetGrid();
    }

    public PlayerStats GetDefaultStatsByClass(CharacterClass charClass) {
        PlayerStats stats = new PlayerStats();

        switch (charClass) {
            case CharacterClass.Warrior:
                stats.strength = 15;
                stats.dexterity = 10;
                stats.intelligence = 5;
                stats.vitality = 12;
                break;

            case CharacterClass.Mage:
                stats.strength = 5;
                stats.dexterity = 10;
                stats.intelligence = 15;
                stats.vitality = 8;
                break;

            case CharacterClass.Archer:
                stats.strength = 10;
                stats.dexterity = 15;
                stats.intelligence = 8;
                stats.vitality = 10;
                break;

            default:
                stats.strength = 10;
                stats.dexterity = 10;
                stats.intelligence = 10;
                stats.vitality = 10;
                break;
        }

        return stats;
    }

}
