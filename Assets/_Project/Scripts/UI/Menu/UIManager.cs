using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private GameObject pauseUI;
    private bool isPaused;

    [SerializeField]
    private GameObject inventoryUI;
    private bool isInventoryOpen;

    [SerializeField]
    private DialogueUI dialogueUI;

    void Start() {
        AudioManager.Instance.PlayMusic(MusicType.Exploration);

        isPaused = false;
        pauseUI.SetActive(isPaused);

        isInventoryOpen = false;
        inventoryUI.SetActive(isInventoryOpen);
    }

    void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (isInventoryOpen) CloseInventory();
            else if (isPaused) Resume();
            else Pause();
        }
        if (Keyboard.current.eKey.wasPressedThisFrame) {
            if (isPaused) return;
            if (isInventoryOpen) CloseInventory();
            else OpenInventory();
        }
    }

    void Resume() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        isPaused = false;
    }

    void Pause() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
        isPaused = true;
    }

    void CloseInventory() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        Time.timeScale = 1f;
        inventoryUI.SetActive(false);
        isInventoryOpen = false;
    }

    void OpenInventory() {
        AudioManager.Instance.PlaySFX(SFX.UIInventoryOpen);
        Time.timeScale = 0f;
        inventoryUI.SetActive(true);
        isInventoryOpen = true;
    }

    public void OnPlayClicked() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        isPaused = false;
    }

    public void OnSaveCharacterClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        dialogueUI.Show("Character saved!");
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        isPaused = false;
        GameManager.Instance.SaveCharacter();
    }

    public void OnSaveGameClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        dialogueUI.Show("Game saved!");
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        isPaused = false;
        GameManager.Instance.SaveGame();
    }

    public void OnExitClicked() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        Time.timeScale = 1f;
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }
}
