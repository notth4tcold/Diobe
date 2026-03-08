using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [SerializeField] private ItemData[] startingItems;

    public GameSaveData gameSaveData { get; private set; }

    public Player player { get; private set; }
    public event Action<Player> OnPlayerReady;

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

    public void NewCharacter(string name, int classID) {
        LoadGameSaveDataForNewCharacter(name, classID);
        SceneManager.LoadScene("Home");
    }

    public void LoadGameSaveDataForNewCharacter(string name, int classID) {
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
            lastSave = DateTime.Now
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
        gameSaveData = new GameSaveData {
            characterSaveData = character,
            playerPosition = Vector2.zero,
            hasPlayerPosition = false,
            mapPosition = Vector2.zero,
            hasMapPosition = false
        };

        SceneManager.LoadScene("Home");
    }

    public void LoadGame(GameSaveData gameSave) {
        gameSaveData = gameSave;

        SceneManager.LoadScene("Home");
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
        gameSaveData.characterSaveData.id = player.id;
        gameSaveData.characterSaveData.playerName = player.playerName;
        gameSaveData.characterSaveData.characterClass = player.characterClass;
        gameSaveData.characterSaveData.money = player.money;
        gameSaveData.characterSaveData.level = player.level;
        gameSaveData.characterSaveData.exp = player.exp;
        gameSaveData.characterSaveData.stats = player.stats;
        gameSaveData.characterSaveData.resources = player.resources;
        gameSaveData.characterSaveData.combat = player.combat;

        gameSaveData.characterSaveData.items = player.BuildInventorySaveData();
        gameSaveData.characterSaveData.equipments = player.BuildEquipmentSaveData();
    }

    public void FillGameSaveData() {
        gameSaveData.playerPosition = LevelManager.Instance.GetPlayerTransform();
        gameSaveData.hasPlayerPosition = true;
        gameSaveData.mapPosition = LevelManager.Instance.GetMapTransform();
        gameSaveData.hasMapPosition = true;
    }

    public void AddInitialItems() {
        foreach (var data in startingItems) {
            InventoryItem item = new InventoryItem(data, 0, 0);
            item.GenerateModifiers(1);
            player.PickupItem(item);
        }
    }

    public void LoadItems(List<InventoryItemSaveData> items) {
        player.ResetGrid();

        foreach (var saveItem in items) {
            ItemData data = ItemDatabase.Instance.Get(saveItem.itemId);
            InventoryItem item = new InventoryItem(data, saveItem.x, saveItem.y) {
                itemLevel = saveItem.itemLevel,
                modifiers = saveItem.modifiers
            };
            player.AddItem(item);
        }
    }

    public void LoadEquipments(List<EquipmentItemSaveData> equipments) {
        player.ResetSlots();

        foreach (var saveItem in equipments) {
            ItemData data = ItemDatabase.Instance.Get(saveItem.itemId);
            InventoryItem item = new InventoryItem(data, 0, 0) {
                itemLevel = saveItem.itemLevel,
                modifiers = saveItem.modifiers
            };

            if (item.data.equipmentType == EquipmentType.Ring) player.EquipRingInSlot(item, saveItem.slot);
            else player.EquipItemToInventory(item);
        }
    }

    public void ResetGameState() {
        gameSaveData = null;
        player.ResetGrid();
        player.ResetSlots();
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

    public void RegisterPlayer(Player player) {
        this.player = player;
        player.Initialize(gameSaveData);
        OnPlayerReady?.Invoke(player);

        if (gameSaveData.isNewPlayer) {
            player.ResetHeathAndMana();
            AddInitialItems();
            gameSaveData.isNewPlayer = false;
            return;
        }

        LoadItems(gameSaveData.characterSaveData.items);
        LoadEquipments(gameSaveData.characterSaveData.equipments);
    }

    public void SubscribeToPlayerReady(Action<Player> callback) {
        OnPlayerReady += callback;
        if (player != null) callback(player);
    }

    public void UnsubscribeFromPlayerReady(Action<Player> callback) {
        OnPlayerReady -= callback;
    }
}
