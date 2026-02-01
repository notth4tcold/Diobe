using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GameSaveData {
    public CharacterSaveData characterSaveData;

    public Vector2 playerPosition;
    public bool hasPlayerPosition;

    public Vector2 mapPosition;
    public bool hasMapPosition;

    [DoNotSerialize]
    public bool isNewPlayer = false;
}
