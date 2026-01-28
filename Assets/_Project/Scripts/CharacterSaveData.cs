
using System;

[Serializable]
public class CharacterSaveData {
    public string id;
    public string playerName;
    public int level;
    public int exp;
    public DateTime lastSave;
    public CharacterClass characterClass;
}

public enum CharacterClass {
    Warrior,
    Archer,
    Mage
}