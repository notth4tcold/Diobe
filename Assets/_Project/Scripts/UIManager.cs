using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private GameObject pauseUI;

    private bool isPaused;

    void Start() {
        isPaused = false;
        pauseUI.SetActive(isPaused);
    }

    void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (isPaused) Resume();
            else Pause();
        }
    }

    void Resume() {
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        isPaused = false;
    }

    void Pause() {
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
        isPaused = true;
    }

    public void OnPlayClicked() {
        Resume();
    }

    public void OnSaveCharacterClicked() {
        CharacterSaveSystem.SaveCharacter(GameManager.Instance.characterSaveData);
    }

    public void OnSaveGameClicked() {
        GameManager.Instance.FillGameSaveData();
        GameSaveSystem.SaveGame(GameManager.Instance.gameSaveData);
    }

    public void OnExitClicked() {
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }
}
