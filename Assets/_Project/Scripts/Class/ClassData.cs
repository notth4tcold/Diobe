using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassData", menuName = "Scriptable Objects/ClassData")]
public class ClassData : ScriptableObject {
    public CharacterClass Class;

    [Header("Primary Attributes")]
    public int Strength;
    public int Dexterity;
    public int Intelligence;
    public int Vitality;
}