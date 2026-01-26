using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerNewGame : MonoBehaviour {

    [SerializeField]
    private TMP_InputField nameInput;

    [SerializeField]
    private TMP_Dropdown classDropdown;

    [SerializeField]
    private TMP_Dropdown importDropdown;

    [SerializeField]
    private GameObject importGameobject;

    List<CharacterData> loadedCharacters;

    void Start() {
        LoadCharactersIntoDropdown();
        importGameobject.SetActive(false);
    }

    void LoadCharactersIntoDropdown() {

        loadedCharacters = CharacterSaveSystem.LoadAllCharacters();

        importDropdown.ClearOptions();
        List<string> options = new() {
            "— Select —"
        };

        foreach (var c in loadedCharacters) {
            options.Add($"{c.playerName} ({c.characterClass})");
        }

        importDropdown.AddOptions(options);
        importDropdown.value = 0;
    }

    public void OnStartGameClicked() {

        if (importDropdown.value > 0 && importDropdown.value <= loadedCharacters.Count) {

            var character = loadedCharacters[importDropdown.value - 1];

            GameManager.Instance.characterData = new CharacterData {
                id = character.id,
                playerName = character.playerName,
                characterClass = character.characterClass
            };

            Debug.Log("Game start with Player " + character.playerName + " as " + character.characterClass);

            SceneManager.LoadScene("Home");
            return;
        }

        string name = nameInput.text;
        string classs = classDropdown.options[classDropdown.value].text;

        Debug.Log("New Player " + name + " as " + classs);

        GameManager.Instance.characterData = new CharacterData {
            id = System.Guid.NewGuid().ToString(),
            playerName = name,
            characterClass = (CharacterClass)classDropdown.value
        };

        SceneManager.LoadScene("Home");
    }

    public void OnImportClicked() {
        importGameobject.SetActive(!importGameobject.activeSelf);
    }

    public void OnBackClicked() {
        SceneManager.LoadScene("MainMenu");
    }
}
