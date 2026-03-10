using UnityEngine;

[CreateAssetMenu(fileName = "ClassDatabase", menuName = "Scriptable Objects/ClassDatabase")]
public class ClassDatabase : ScriptableObject {
    public ClassData[] classes;

    public ClassData Get(CharacterClass charClass) {
        foreach (var c in classes) {
            if (c.Class == charClass) return c;
        }

        return null;
    }
}
