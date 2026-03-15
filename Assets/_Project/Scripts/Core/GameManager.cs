using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [SerializeField] private ItemData[] startingItems;

    public GameSaveData gameSaveData { get; private set; }

    public Player Player { get; private set; }
    public event Action<Player> OnPlayerReady;

    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private ModifierGeneratorDatabase modifierGeneratorDatabase;

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
        ClassData classData = classDatabase.Get((CharacterClass)classID);

        List<StatValue> baseStats = new() {
            new StatValue { stat = StatType.Strength, value = classData.Strength },
            new StatValue { stat = StatType.Dexterity, value = classData.Dexterity },
            new StatValue { stat = StatType.Intelligence, value = classData.Intelligence },
            new StatValue { stat = StatType.Vitality, value = classData.Vitality },
        };

        var characterSaveData = new CharacterSaveData {
            id = Guid.NewGuid().ToString(),
            playerName = name,
            characterClass = (CharacterClass)classID,
            money = 0,
            level = 1,
            exp = 0,
            baseStats = baseStats,
            health = 0,
            mana = 0,
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
        // TODO load items da fase e facingRight do player

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
        gameSaveData.characterSaveData.id = Player.id;
        gameSaveData.characterSaveData.playerName = Player.playerName;
        gameSaveData.characterSaveData.characterClass = Player.characterClass;
        gameSaveData.characterSaveData.money = Player.money;
        gameSaveData.characterSaveData.level = Player.level;
        gameSaveData.characterSaveData.exp = Player.exp;

        gameSaveData.characterSaveData.baseStats = Player.BuildBaseStatsSaveData();
        gameSaveData.characterSaveData.health = Player.resources.health;
        gameSaveData.characterSaveData.mana = Player.resources.mana;

        gameSaveData.characterSaveData.items = Player.BuildInventorySaveData();
        gameSaveData.characterSaveData.equipments = Player.BuildEquipmentSaveData();
    }

    public void FillGameSaveData() {
        gameSaveData.playerPosition = LevelManager.Instance.GetPlayerTransform();
        gameSaveData.hasPlayerPosition = true;
        gameSaveData.mapPosition = LevelManager.Instance.GetMapTransform();
        gameSaveData.hasMapPosition = true;
    }

    public void AddInitialItems() {
        foreach (var data in startingItems) {
            Item item = new Item(data, 0, 0);
            item.GenerateModifiers(Player.level, LootQuality.Normal);
            Player.PickupItem(item);
        }
    }

    public void LoadItems(List<InventoryItemSaveData> items) {
        Player.ResetGrid();

        foreach (var saveItem in items) {
            ItemData data = itemDatabase.Get(saveItem.itemId);
            Item item = new Item(data, saveItem.x, saveItem.y) {
                itemLevel = saveItem.itemLevel,
                modifiers = saveItem.modifiers ?? new List<ItemModifier>(),
            };
            Player.PlaceItem(item);
        }
    }

    public void LoadEquipments(List<EquipmentItemSaveData> equipments) {
        Player.ResetSlots();

        foreach (var saveItem in equipments) {
            ItemData data = itemDatabase.Get(saveItem.itemId);
            Item item = new Item(data, 0, 0) {
                itemLevel = saveItem.itemLevel,
                modifiers = saveItem.modifiers ?? new List<ItemModifier>(),
            };

            if (item.data.equipmentType == EquipmentType.Ring) Player.EquipRingInSlot(item, saveItem.slot);
            else Player.EquipItem(item);
        }
    }

    public void ResetGameState() {
        gameSaveData = null;
        Player.ResetGrid();
        Player.ResetSlots();
    }

    public void RegisterPlayer(Player player) {
        Player = player;
        player.Initialize(gameSaveData);
        OnPlayerReady?.Invoke(player);

        if (gameSaveData.isNewPlayer) {
            player.ResetHealthAndMana();
            AddInitialItems();
            gameSaveData.isNewPlayer = false;
            return;
        }

        LoadItems(gameSaveData.characterSaveData.items);
        LoadEquipments(gameSaveData.characterSaveData.equipments);
    }

    public ModifierGeneratorDatabase GetModifierGeneratorDatabase() {
        return modifierGeneratorDatabase;
    }

    public void SubscribeToPlayerReady(Action<Player> callback) {
        OnPlayerReady += callback;
        if (Player != null) callback(Player);
    }

    public void UnsubscribeFromPlayerReady(Action<Player> callback) {
        OnPlayerReady -= callback;
    }
}
