using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public static UIManager Instance { get; private set; }

    private PlayerInputHandler inputHandler;

    [SerializeField] private GameObject pauseUI;
    private bool isPaused;

    [SerializeField] private GameObject characterUI;
    private bool isCharacterUIOpen;

    [SerializeField] private DialogueUI dialogueUI;

    [SerializeField] private InventoryUI inventoryUI;

    public Transform DragLayer;

    void Awake() {
        Instance = this;
    }

    void Start() {
        AudioManager.Instance.PlayMusic(MusicType.Exploration);

        isPaused = false;
        pauseUI.SetActive(isPaused);

        isCharacterUIOpen = false;
        characterUI.SetActive(isCharacterUIOpen);
    }

    void InitializeInput() {
        inputHandler.OnPauseUIPressed += HandlePauseUI;
        inputHandler.OnCharacterUIPressed += HandleCharacterUI;
    }

    void HandlePauseUI() {
        if (dialogueUI.IsOpened()) return;
        if (isCharacterUIOpen) CloseCharacterUI();
        else if (isPaused) Resume();
        else Pause();
    }

    void HandleCharacterUI() {
        if (dialogueUI.IsOpened()) return;
        if (isPaused) return;
        if (isCharacterUIOpen) CloseCharacterUI();
        else OpenCharacterUI();
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

    void CloseCharacterUI() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        Time.timeScale = 1f;
        characterUI.SetActive(false);
        isCharacterUIOpen = false;
    }

    void OpenCharacterUI() {
        AudioManager.Instance.PlaySFX(SFX.UICharacterOpen);
        Time.timeScale = 0f;
        characterUI.SetActive(true);
        isCharacterUIOpen = true;
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

    public bool IsUIBlocking => isPaused || isCharacterUIOpen;

    void OnDestroy() {
        if (inputHandler == null) return;
        inputHandler.OnPauseUIPressed -= HandlePauseUI;
        inputHandler.OnCharacterUIPressed -= HandleCharacterUI;
    }

    public Vector2 DefaultInventoryCelltSize => inventoryUI.DefaultCellSize;

    void OnEnable() {
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void OnDisable() {
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    private void HandlePlayerReady(Player p) {
        if (inputHandler != null) return;
        inputHandler = p.GetComponent<PlayerInputHandler>();
        InitializeInput();
    }
}
