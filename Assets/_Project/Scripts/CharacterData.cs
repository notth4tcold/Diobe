
[System.Serializable]
public class CharacterData {
    public string id;
    public string playerName;
    public CharacterClass characterClass;
}

public enum CharacterClass {
    Warrior,
    Archer,
    Mage
}