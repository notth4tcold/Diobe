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

    public void InitializeResourcesAndCombat() {
        resources.Initialize(stats);
        combat.Initialize(stats);
    }

    public void ResetHeathAndMana() {
        resources.health = resources.MaxHealth;
        resources.mana = resources.MaxMana;
    }

    // Aqui você pode criar métodos de regeneração, level up, etc.

    //equipamento (opcional, mas comum)
}

[System.Serializable]
public enum CharacterClass {
    Warrior,
    Archer,
    Mage
}