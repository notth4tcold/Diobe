using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerNewGame : MonoBehaviour {

    [SerializeField]
    private TMP_InputField nameInput;

    [SerializeField]
    private GameObject nameGameobject;

    [SerializeField]
    private TMP_Dropdown classDropdown;

    [SerializeField]
    private GameObject classGameobject;

    [SerializeField]
    private TMP_Dropdown importDropdown;

    [SerializeField]
    private GameObject importGameobject;

    List<CharacterSaveData> loadedCharacters;

    [SerializeField]
    private GameObject startGameobject;

    [SerializeField]
    private GameObject importStartGameobject;

    void Start() {
        LoadCharactersIntoDropdown();
        importGameobject.SetActive(false);
        importStartGameobject.SetActive(false);
    }

    void LoadCharactersIntoDropdown() {
        loadedCharacters = GameManager.Instance.GetAllCharacters();

        importDropdown.ClearOptions();
        List<string> options = new() {
            "— Select —"
        };

        foreach (var c in loadedCharacters) {
            options.Add($"{c.playerName} ({c.characterClass}) - Lv {c.level}");
        }

        importDropdown.AddOptions(options);
        importDropdown.value = 0;
    }

    public void OnStartGameClicked() {
        if (nameInput.text != "") {
            string name = nameInput.text;
            string classs = classDropdown.options[classDropdown.value].text;

            Debug.Log("New Player " + name + " as " + classs);

            GameManager.Instance.NewCharacter(name, classDropdown.value);

            SceneManager.LoadScene("Home");
        }
    }

    public void OnImportStartClicked() {
        if (importDropdown.value > 0 && importDropdown.value <= loadedCharacters.Count) {
            var character = loadedCharacters[importDropdown.value - 1];

            GameManager.Instance.ImportCharacter(character);

            Debug.Log("Game start with Player " + character.playerName + " as " + character.characterClass);

            SceneManager.LoadScene("Home");
        }
    }

    public void OnImportClicked() {
        nameGameobject.SetActive(!nameGameobject.activeSelf);
        classGameobject.SetActive(!classGameobject.activeSelf);
        startGameobject.SetActive(!startGameobject.activeSelf);
        importGameobject.SetActive(!importGameobject.activeSelf);
        importStartGameobject.SetActive(!importStartGameobject.activeSelf);
    }

    public void OnBackClicked() {
        SceneManager.LoadScene("MainMenu");
    }
}
