[System.Serializable]
public class PlayerStats {
    public int strength; //Aumenta dano físico, aumentar armadura?, Requisitos de armas pesadas,
    public int dexterity; //Aumenta chance de acerto, Chance de esquiva, Chance de crítico, Requisitos de armas leves
    public int intelligence; // Aumenta dano mágico, Mana maxima, Regeneração de Mana, Poder de feitiços
    public int vitality; //Vida máxima, Regeneração de vida

    public float baseMoveSpeed = 5f;
    public float bonusMoveSpeed = 0f;

    public float MoveSpeed => baseMoveSpeed + bonusMoveSpeed;
}