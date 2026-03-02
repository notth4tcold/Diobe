using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {
    public string id;
    public string playerName;
    public CharacterClass characterClass;

    public int money;
    public int level;
    public int exp;

    public PlayerStats stats = new();
    public PlayerResources resources = new();
    public PlayerCombat combat = new();

    public event Action<float> OnHealthPercentChanged;
    public event Action<float> OnManaPercentChanged;

    [SerializeField] private SpriteResolver chestResolver;
    [SerializeField] private SpriteResolver headResolver;
    [SerializeField] private SpriteResolver armLResolver;
    [SerializeField] private SpriteResolver armRResolver;
    [SerializeField] private SpriteResolver legLResolver;
    [SerializeField] private SpriteResolver legRResolver;

    [SerializeField] private PlayerWeapon playerWeapon;

    public InventoryGrid InventoryGrid { get; private set; }
    public EquipmentInventory EquipmentInventory { get; private set; }

    void Awake() {
        EquipmentInventory = GetComponent<EquipmentInventory>();
        InventoryGrid = GetComponent<InventoryGrid>();
        EquipmentInventory.Initialize();
        InventoryGrid.Initialize();
        GameManager.Instance.RegisterPlayer(this);
    }

    void Start() {
        CameraManager.Instance.SetTarget(transform);

        chestResolver.SetCategoryAndLabel("Chest", "Default");
        headResolver.SetCategoryAndLabel("Head", "Default");
        armLResolver.SetCategoryAndLabel("Arm_L", "Default");
        armRResolver.SetCategoryAndLabel("Arm_R", "Default");
        legLResolver.SetCategoryAndLabel("Leg_L", "Default");
        legRResolver.SetCategoryAndLabel("Leg_R", "Default");
    }

    void Update() {
        Regenerate(Time.deltaTime);
    }

    public void Initialize(GameSaveData save) {
        id = save.characterSaveData.id;
        playerName = save.characterSaveData.playerName;
        characterClass = save.characterSaveData.characterClass;
        money = save.characterSaveData.money;
        level = save.characterSaveData.level;
        exp = save.characterSaveData.exp;
        stats = save.characterSaveData.stats;
        resources = save.characterSaveData.resources;
        combat = save.characterSaveData.combat;

        InitializeResourcesAndCombat();
        AddOnItemEquippedEvent();
    }

    public void InitializeResourcesAndCombat() {
        resources.Initialize(stats);
        combat.Initialize(stats);
    }

    // Resources
    public void SetHealth(int value) {
        resources.health = Mathf.Clamp(value, 0, resources.MaxHealth);
        OnHealthPercentChanged?.Invoke(HealthPercent);
    }

    public void SetMana(int value) {
        resources.mana = Mathf.Clamp(value, 0, resources.MaxMana);
        OnManaPercentChanged?.Invoke(ManaPercent);
    }

    public void TakeDamage(int amount) {
        if (Dodge || Block) return;
        amount -= combat.armor;

        // TODO Validar se o tipo do dano e adicionar resistancia
        //fireRes
        //iceRes
        //lightningRes
        //poisonRes
        //magicRes

        SetHealth(resources.health - amount);
    }

    public void SpendMana(int amount) {
        SetMana(resources.mana - amount);
    }

    public void Heal(int amount) {
        SetHealth(resources.health + amount);
    }

    public void RestoreMana(int amount) {
        SetMana(resources.mana + amount);
    }

    public void ResetHeathAndMana() {
        SetHealth(resources.MaxHealth);
        SetMana(resources.MaxMana);
    }

    float healthRegenBuffer;
    float manaRegenBuffer;

    void Regenerate(float delta) {
        healthRegenBuffer += resources.HealthRegen * delta;
        manaRegenBuffer += resources.ManaRegen * delta;

        int healthToAdd = Mathf.FloorToInt(healthRegenBuffer);
        int manaToAdd = Mathf.FloorToInt(manaRegenBuffer);

        if (healthToAdd > 0) {
            Heal(healthToAdd);
            healthRegenBuffer -= healthToAdd;
        }

        if (manaToAdd > 0) {
            RestoreMana(manaToAdd);
            manaRegenBuffer -= manaToAdd;
        }
    }

    public float HealthPercent => resources.MaxHealth == 0 ? 0 : (float)resources.health / resources.MaxHealth;
    public float ManaPercent => resources.MaxMana == 0 ? 0 : (float)resources.mana / resources.MaxMana;


    // Combat

    public int DoDamage() {

        // TODO Validar o tipo do atack do player e retornar + dano elemental
        // FireDamage
        // IceDamage
        // LightningDamage
        // PoisonDamage

        if (Hit) {
            if (Crit) return Mathf.RoundToInt(combat.Damage * combat.critMultiplier);
            return combat.Damage;
        }

        return 0;
    }

    public bool Dodge => Random.value < combat.dodgeChance;
    public bool Block => Random.value < combat.blockChance;
    public bool Crit => Random.value < combat.critChance;
    public bool Hit => Random.value < combat.hitChance;

    //Inventory
    public List<InventoryItemSaveData> BuildInventorySaveData() => InventoryGrid.BuildSaveData();
    public void ResetGrid() => InventoryGrid.ResetGrid();
    public void AddItem(InventoryItem item) => InventoryGrid.PlaceItem(item, item.x, item.y);
    public bool SpawnItem(InventoryItem item) => InventoryGrid.SpawnItem(item);

    public bool PickupItem(ItemData data) {
        InventoryItem item = new InventoryItem(data, 0, 0);

        if (item.data.type == ItemType.Weapon && !HasWeapon) {
            return EquipItemToInventory(item);
        }

        return SpawnItem(item);
    }

    public void DropItem(InventoryItem item) => LevelManager.Instance.SpawnItem(transform.position, item.data);

    // Equipment
    public bool HasWeapon => EquipmentInventory.HasWeapon;
    public bool EquipItemToInventory(InventoryItem item) => EquipmentInventory.EquipItem(item);
    public bool EquipRingInSlot(InventoryItem item, RingSlot slot) => EquipmentInventory.EquipRingInSlot(item, slot);

    public void EquipWeapon(InventoryItem item) => playerWeapon.Equip(item.data);
    public void UnequipWeapon() => playerWeapon.Unequip();

    public List<EquipmentItemSaveData> BuildEquipmentSaveData() => EquipmentInventory.BuildSaveData();
    public void ResetSlots() => EquipmentInventory.ResetSlots();

    void AddOnItemEquippedEvent() {
        EquipmentInventory.OnItemEquipped += HandleItemEquipped;
        EquipmentInventory.OnItemUnequipped += HandleItemUnequipped;
    }
    void OnDestroy() {
        EquipmentInventory.OnItemEquipped -= HandleItemEquipped;
        EquipmentInventory.OnItemUnequipped -= HandleItemUnequipped;
    }

    void HandleItemEquipped(InventoryItem item) {
        if (item.data.equipmentType == EquipmentType.MainHand) EquipWeapon(item);
    }
    void HandleItemUnequipped(InventoryItem item) {
        if (item.data.equipmentType == EquipmentType.MainHand) UnequipWeapon();
    }
}

[System.Serializable]
public enum CharacterClass {
    Warrior,
    Archer,
    Mage
}