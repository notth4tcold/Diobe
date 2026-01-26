using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private GameObject pauseUI;

    void Start() {
        pauseUI.SetActive(false);
    }

    void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            pauseUI.SetActive(!pauseUI.activeSelf);
        }
    }

    public void OnPlayClicked() {
        pauseUI.SetActive(false);
    }

    public void OnSaveCharacterClicked() {
        CharacterSaveSystem.SaveCharacter(GameManager.Instance.characterData);
    }

    public void OnExitClicked() {
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }
}
