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

    void Start() {
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
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        isPaused = false;
    }

    void Pause() {
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
        isPaused = true;
    }

    void CloseInventory() {
        Time.timeScale = 1f;
        inventoryUI.SetActive(false);
        isInventoryOpen = false;
    }

    void OpenInventory() {
        Time.timeScale = 0f;
        inventoryUI.SetActive(true);
        isInventoryOpen = true;
    }

    public void OnPlayClicked() {
        Resume();
    }

    public void OnSaveCharacterClicked() {
        GameManager.Instance.SaveCharacter();
    }

    public void OnSaveGameClicked() {
        GameManager.Instance.SaveGame();
    }

    public void OnExitClicked() {
        Time.timeScale = 1f;
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }
}
