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

    [SerializeField]
    private TMP_Text importText;

    List<CharacterSaveData> loadedCharacters;

    [SerializeField]
    private GameObject startGameobject;

    [SerializeField]
    private GameObject importStartGameobject;

    [SerializeField]
    private DialogueUI dialogueUI;

    void Start() {
        LoadCharactersIntoDropdown();
        importGameobject.SetActive(false);
        importStartGameobject.SetActive(false);
        importText.text = "Import";
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
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        if (nameInput.text != "") {
            string name = nameInput.text;
            string classs = classDropdown.options[classDropdown.value].text;
            GameManager.Instance.NewCharacter(name, classDropdown.value);

            Debug.Log("New Player " + name + " as " + classs);

            SceneManager.LoadScene("Home");
        } else {
            dialogueUI.Show("Please, type your player name!");
        }
    }

    public void OnImportStartClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        if (importDropdown.value > 0 && importDropdown.value <= loadedCharacters.Count) {
            var character = loadedCharacters[importDropdown.value - 1];
            GameManager.Instance.ImportCharacter(character);

            Debug.Log("Game start with Player " + character.playerName + " as " + character.characterClass);

            SceneManager.LoadScene("Home");
        } else {
            dialogueUI.Show("Please, select your character!");
        }
    }

    public void OnImportClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);

        if (importGameobject.activeSelf) importText.text = "Import";
        else importText.text = "New Player";

        nameGameobject.SetActive(!nameGameobject.activeSelf);
        classGameobject.SetActive(!classGameobject.activeSelf);
        startGameobject.SetActive(!startGameobject.activeSelf);
        importGameobject.SetActive(!importGameobject.activeSelf);
        importStartGameobject.SetActive(!importStartGameobject.activeSelf);
    }

    public void OnBackClicked() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        SceneManager.LoadScene("MainMenu");
    }
}
