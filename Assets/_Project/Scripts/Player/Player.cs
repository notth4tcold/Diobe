using System;
using UnityEngine;

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

    void Update() {
        Regenerate(Time.deltaTime);
    }

    public void InitializeResourcesAndCombat() {
        resources.Initialize(stats);
        combat.Initialize(stats);
    }

    public void SetHealth(int value) {
        resources.health = Mathf.Clamp(value, 0, resources.MaxHealth);
        OnHealthPercentChanged?.Invoke(HealthPercent);
    }

    public void SetMana(int value) {
        resources.mana = Mathf.Clamp(value, 0, resources.MaxMana);
        OnManaPercentChanged?.Invoke(ManaPercent);
    }

    public void TakeDamage(int amount) {
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

    //equipamento (opcional, mas comum)
}

[System.Serializable]
public enum CharacterClass {
    Warrior,
    Archer,
    Mage
}