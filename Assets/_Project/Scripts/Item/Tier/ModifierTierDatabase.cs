using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierTierDatabase", menuName = "Scriptable Objects/ModifierTierDatabase")]
public class ModifierTierDatabase : ScriptableObject {
    public List<ModifierTierData> tiers;

    public ModifierTierData GetTierData(int level) {
        foreach (var t in tiers) {
            if (t.InRange(level)) return t;
        }

        return null;
    }
}