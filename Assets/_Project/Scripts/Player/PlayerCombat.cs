using UnityEngine;

[System.Serializable]
public class PlayerCombat {
    //Defence
    public int armor;
    public float dodgeChance;
    public float blockChance; //escudo

    public int fireRes;
    public int iceRes;
    public int lightningRes;
    public int poisonRes;
    public int magicRes; //resistência a dano mágico puro, não elemental

    //Attack
    public int attack;
    public int attackSpeed;
    public float hitChance;
    public float critChance;

    public int weaponAttack;
    public int weaponFireAttack;
    public int weaponIceAttack;
    public int weaponLightningAttack;
    public int weaponPoisonAttack;

    public float strengthMultiplier = 1f;

    private PlayerStats stats;

    public void Initialize(PlayerStats stats) {
        this.stats = stats;
    }

    public int WeaponDamage => attack + weaponAttack;
    public int Damage => WeaponDamage + Mathf.RoundToInt(stats.strength * strengthMultiplier);
    public int FireDamage => Damage + weaponFireAttack;
    public int IceDamage => Damage + weaponIceAttack;
    public int LightningDamage => Damage + weaponLightningAttack;
    public int PoisonDamage => Damage + weaponPoisonAttack;
}