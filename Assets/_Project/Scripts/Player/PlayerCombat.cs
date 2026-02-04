using UnityEngine;

[System.Serializable]
public class PlayerCombat {
    private PlayerStats stats;

    public void Initialize(PlayerStats stats) {
        this.stats = stats;
    }

    //Defence
    public int armor;
    public float dodgeChance = 0.01f; //1% de chance de desviar
    public float blockChance = 0.3f; //30% de chance do escudo bloquear

    public int fireRes;
    public int iceRes;
    public int lightningRes;
    public int poisonRes;
    public int magicRes; //resistência a dano mágico puro, não elemental

    //Attack
    public int attack = 10;
    public int attackSpeed;

    public float hitChance = 0.7f; //70% de chance de acertar o hit
    public float critChance = 0.1f; //1% de chance de critico
    public float critMultiplier = 2;

    public int weaponAttack;
    public int weaponFireAttack;
    public int weaponIceAttack;
    public int weaponLightningAttack;
    public int weaponPoisonAttack;

    public float strengthMultiplier = 1f;

    public int WeaponDamage => attack + weaponAttack;
    public int Damage => WeaponDamage + Mathf.RoundToInt(stats.strength * strengthMultiplier);
    public int FireDamage => Damage + weaponFireAttack;
    public int IceDamage => Damage + weaponIceAttack;
    public int LightningDamage => Damage + weaponLightningAttack;
    public int PoisonDamage => Damage + weaponPoisonAttack;
}