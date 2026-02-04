using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerLoadGame : MonoBehaviour {


    [SerializeField]
    private TMP_Dropdown loadDropdown;

    [SerializeField]
    private DialogueUI dialogueUI;

    List<GameSaveData> loadedGameSaves;

    void Start() {
        LoadGameSavesIntoDropdown();
    }

    void LoadGameSavesIntoDropdown() {
        loadedGameSaves = GameManager.Instance.GetAllGameSaves();

        loadDropdown.ClearOptions();
        List<string> options = new() {
            "— Select —"
        };

        foreach (var g in loadedGameSaves) {
            options.Add($"{g.characterSaveData.playerName} ({g.characterSaveData.characterClass}) - Lv {g.characterSaveData.level}");
        }

        loadDropdown.AddOptions(options);
        loadDropdown.value = 0;
    }

    public void OnLoadGameClicked() {
        if (loadDropdown.value > 0 && loadDropdown.value <= loadedGameSaves.Count) {
            AudioManager.Instance.PlaySFX(SFX.UIButton);

            var gameSave = loadedGameSaves[loadDropdown.value - 1];
            GameManager.Instance.LoadGame(gameSave);

            Debug.Log("Game start with Player " + gameSave.characterSaveData.playerName + " as " + gameSave.characterSaveData.characterClass);

            SceneManager.LoadScene("Home");
        } else {
            dialogueUI.Show("Please, select your save!");
        }
    }

    public void OnBackClicked() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        SceneManager.LoadScene("MainMenu");
    }
}
