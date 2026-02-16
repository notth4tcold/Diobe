using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public static UIManager Instance { get; private set; }

    private PlayerInputHandler inputHandler;

    [SerializeField] private GameObject pauseUI;
    private bool isPaused;

    [SerializeField] private GameObject inventoryUI;
    private bool isInventoryOpen;

    [SerializeField] private DialogueUI dialogueUI;

    void Awake() {
        Instance = this;
    }

    void Start() {
        AudioManager.Instance.PlayMusic(MusicType.Exploration);

        isPaused = false;
        pauseUI.SetActive(isPaused);

        isInventoryOpen = false;
        inventoryUI.SetActive(isInventoryOpen);

        InitializeInput();
    }

    void InitializeInput() {
        var player = LevelManager.Instance.GetPlayer();
        if (player == null) return;

        inputHandler = player.GetComponent<PlayerInputHandler>();
        if (inputHandler == null) return;

        inputHandler.OnPausePressed += HandlePause;
        inputHandler.OnInventoryPressed += HandleInventory;
    }

    void HandlePause() {
        if (dialogueUI.IsOpened()) return;
        if (isInventoryOpen) CloseInventory();
        else if (isPaused) Resume();
        else Pause();
    }

    void HandleInventory() {
        if (dialogueUI.IsOpened()) return;
        if (isPaused) return;
        if (isInventoryOpen) CloseInventory();
        else OpenInventory();
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
        GameManager.Instance.SaveCharacter();
    }

    public void OnSaveGameClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        dialogueUI.Show("Game saved!");
        GameManager.Instance.SaveGame();
    }

    public void OnExitClicked() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        Time.timeScale = 1f;
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }

    public bool IsUIBlocking => isPaused || isInventoryOpen;

    void OnDestroy() {
        inputHandler.OnPausePressed -= HandlePause;
        inputHandler.OnInventoryPressed -= HandleInventory;
    }
}
