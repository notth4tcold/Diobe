using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

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
    public EquipmentSlots EquipmentInventory { get; private set; }

    void Awake() {
        EquipmentInventory = GetComponent<EquipmentSlots>();
        InventoryGrid = GetComponent<InventoryGrid>();
        EquipmentInventory.Initialize();
        InventoryGrid.Initialize();
        GameManager.Instance.RegisterPlayer(this);
    }

    void Start() {
        CameraManager.Instance.SetTarget(transform);

        // TODO ajustar sprite para cada tipo de armadura que adicionar, por enquanto nao temos outra alem da default
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

        // Stats
        stats = new();
        foreach (var stat in save.characterSaveData.baseStats) {
            stats.SetBase(stat.stat, stat.value);
        }

        // Resources
        resources = new PlayerResources {
            health = save.characterSaveData.health,
            mana = save.characterSaveData.mana
        };
        resources.SetStats(stats);

        // Combat
        combat = new PlayerCombat();
        combat.SetStats(stats);

        AddOnItemEquippedEvent();
        AddOnStatsChangedEvent();
    }

    // Stats
    public List<StatValue> BuildBaseStatsSaveData() => stats.BuildBaseStatsSaveData();


    // Resources
    public void SetHealth(int value) {
        resources.health = Mathf.Clamp(value, 0, resources.MaxHealth);
        OnHealthPercentChanged?.Invoke(HealthPercent);
    }

    public void SetMana(int value) {
        resources.mana = Mathf.Clamp(value, 0, resources.MaxMana);
        OnManaPercentChanged?.Invoke(ManaPercent);
    }

    public float HealthPercent => resources.MaxHealth == 0 ? 0 : (float)resources.health / resources.MaxHealth;
    public float ManaPercent => resources.MaxMana == 0 ? 0 : (float)resources.mana / resources.MaxMana;

    public void TakeDamage(DamageInfo dmg) {
        if (dmg.canDodge && combat.RollDodge()) return;
        if (dmg.canBlock && combat.RollBlock()) return;

        int totalDamage = 0;

        // Physical
        int physical = Mathf.Max(0, dmg.physical - combat.Armor);
        totalDamage += physical;

        // Elemental
        totalDamage += ApplyResistance(dmg.fire, combat.FireRes);
        totalDamage += ApplyResistance(dmg.ice, combat.IceRes);
        totalDamage += ApplyResistance(dmg.lightning, combat.LightningRes);
        totalDamage += ApplyResistance(dmg.poison, combat.PoisonRes);
        totalDamage += ApplyResistance(dmg.magic, combat.MagicRes);

        SetHealth(resources.health - totalDamage);
    }

    int ApplyResistance(int damage, float resistancePercent) {
        float multiplier = 1f - resistancePercent / 100f;
        return Mathf.RoundToInt(damage * multiplier);
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

    public void ResetHealthAndMana() {
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


    // Combat
    public DamageInfo DoDamage() => combat.CreateDamage();


    //Inventory
    public List<InventoryItemSaveData> BuildInventorySaveData() => InventoryGrid.BuildSaveData();
    public void ResetGrid() => InventoryGrid.ResetGrid();
    public void AddItem(InventoryItem item) => InventoryGrid.PlaceItem(item, item.x, item.y);
    public bool SpawnItem(InventoryItem item) => InventoryGrid.SpawnItem(item);

    public bool PickupItem(InventoryItem item) {
        if (item.data.type == ItemType.Weapon && !HasWeapon) {
            return EquipItemToInventory(item);
        }

        return SpawnItem(item);
    }

    public void DropItem(InventoryItem item) => LevelManager.Instance.SpawnItem(transform.position, item);

    // Equipment
    public bool HasWeapon => EquipmentInventory.HasWeapon;
    public bool EquipItemToInventory(InventoryItem item) => EquipmentInventory.EquipItem(item);
    public bool EquipRingInSlot(InventoryItem item, RingSlot slot) => EquipmentInventory.EquipRingInSlot(item, slot);

    public void EquipWeapon(InventoryItem item) => playerWeapon.Equip(item.data);
    public void UnequipWeapon() => playerWeapon.Unequip();

    public List<EquipmentItemSaveData> BuildEquipmentSaveData() => EquipmentInventory.BuildSaveData();
    public void ResetSlots() => EquipmentInventory.ResetSlots();

    void AddOnItemEquippedEvent() {
        EquipmentSlots.OnItemEquipped += HandleItemEquipped;
        EquipmentSlots.OnItemUnequipped += HandleItemUnequipped;
    }

    void AddOnStatsChangedEvent() {
        stats.OnStatsChanged += HandleStatsChanged;
    }

    void OnDestroy() {
        EquipmentSlots.OnItemEquipped -= HandleItemEquipped;
        EquipmentSlots.OnItemUnequipped -= HandleItemUnequipped;
        stats.OnStatsChanged -= HandleStatsChanged;
    }

    void HandleItemEquipped(InventoryItem item) {
        if (item.data.equipmentType == EquipmentType.MainHand) EquipWeapon(item);

        foreach (var mod in item.modifiers) stats.AddEquipment(mod.stat, mod.value);
    }
    void HandleItemUnequipped(InventoryItem item) {
        if (item.data.equipmentType == EquipmentType.MainHand) UnequipWeapon();

        foreach (var mod in item.modifiers) stats.RemoveEquipment(mod.stat, mod.value);
    }

    void HandleStatsChanged() {
        SetHealth(resources.health);
        SetMana(resources.mana);
    }
}