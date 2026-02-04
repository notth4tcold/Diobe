using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [Header("Sources")]
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource uiSource;
    [SerializeField] AudioSource musicSource;

    [Header("Music")]
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip explorationMusic;
    [SerializeField] AudioClip combatMusic;

    [Header("Player / Combat")]
    [SerializeField] AudioClip playerHit;
    [SerializeField] AudioClip playerUseHealth;
    [SerializeField] AudioClip playerUseMana;
    [SerializeField] AudioClip playerNoHealth;
    [SerializeField] AudioClip playerNoMana;
    [SerializeField] AudioClip playerDie;
    [SerializeField] AudioClip playerLevelUp;
    [SerializeField] AudioClip playerCompleteQuest;
    [SerializeField] AudioClip playerPickupGold;
    [SerializeField] AudioClip playerJump;
    [SerializeField] AudioClip playerLand;
    [SerializeField] AudioClip slash;
    [SerializeField] AudioClip fireHit;
    [SerializeField] AudioClip iceHit;
    [SerializeField] AudioClip critHit;
    [SerializeField] AudioClip blockHit;

    [Header("UI")]
    [SerializeField] AudioClip uiButton;
    [SerializeField] AudioClip uiConfirm;
    [SerializeField] AudioClip uiCancel;
    [SerializeField] AudioClip uiPauseOpen;
    [SerializeField] AudioClip uiPauseClose;
    [SerializeField] AudioClip uiInventoryOpen;
    [SerializeField] AudioClip uiInventoryClose;
    [SerializeField] AudioClip uiNewGame;
    [SerializeField] AudioClip uiSaveGame;
    [SerializeField] AudioClip uiEquipItem;
    [SerializeField] AudioClip uiUnequipItem;

    [Header("Volume")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;


    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource.loop = true;
        UpdateVolumes();
    }

    public void UpdateVolumes() {
        musicSource.volume = masterVolume * musicVolume;
        sfxSource.volume = masterVolume * sfxVolume;
        uiSource.volume = masterVolume * uiVolume;
    }

    public void PlaySFX(SFX sfx) {
        switch (sfx) {
            case SFX.PlayerHit: sfxSource.PlayOneShot(playerHit); break;
            case SFX.PlayerUseHealth: sfxSource.PlayOneShot(playerUseHealth); break;
            case SFX.PlayerUseMana: sfxSource.PlayOneShot(playerUseMana); break;
            case SFX.PlayerNoHealth: sfxSource.PlayOneShot(playerNoHealth); break;
            case SFX.PlayerNoMana: sfxSource.PlayOneShot(playerNoMana); break;
            case SFX.PlayerDie: sfxSource.PlayOneShot(playerDie); break;
            case SFX.PlayerLevelUp: sfxSource.PlayOneShot(playerLevelUp); break;
            case SFX.PlayerCompleteQuest: sfxSource.PlayOneShot(playerCompleteQuest); break;
            case SFX.PlayerPickupGold: sfxSource.PlayOneShot(playerPickupGold); break;
            case SFX.PlayerJump: sfxSource.PlayOneShot(playerJump); break;
            case SFX.PlayerLand: sfxSource.PlayOneShot(playerLand); break;
            case SFX.Slash: sfxSource.PlayOneShot(slash); break;
            case SFX.FireHit: sfxSource.PlayOneShot(fireHit); break;
            case SFX.IceHit: sfxSource.PlayOneShot(iceHit); break;
            case SFX.CritHit: sfxSource.PlayOneShot(critHit); break;
            case SFX.BlockHit: sfxSource.PlayOneShot(blockHit); break;

            case SFX.UIButton: uiSource.PlayOneShot(uiButton); break;
            case SFX.UIConfirm: uiSource.PlayOneShot(uiConfirm); break;
            case SFX.UICancel: uiSource.PlayOneShot(uiCancel); break;
            case SFX.UIPauseOpen: uiSource.PlayOneShot(uiPauseOpen); break;
            case SFX.UIPauseClose: uiSource.PlayOneShot(uiPauseClose); break;
            case SFX.UIInventoryOpen: uiSource.PlayOneShot(uiInventoryOpen); break;
            case SFX.UIInventoryClose: uiSource.PlayOneShot(uiInventoryClose); break;
            case SFX.UINewGame: uiSource.PlayOneShot(uiNewGame); break;
            case SFX.UISaveGame: uiSource.PlayOneShot(uiSaveGame); break;
            case SFX.UIEquipItem: uiSource.PlayOneShot(uiEquipItem); break;
            case SFX.UIUnequipItem: uiSource.PlayOneShot(uiUnequipItem); break;
        }
    }

    public void PlayMusic(MusicType type) {
        AudioClip clip = type switch {
            MusicType.Menu => menuMusic,
            MusicType.Exploration => explorationMusic,
            MusicType.Combat => combatMusic,
            _ => null
        };

        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }
}


public enum SFX {
    // Player
    PlayerHit,
    PlayerUseHealth,
    PlayerUseMana,
    PlayerNoHealth,
    PlayerNoMana,
    PlayerDie,
    PlayerLevelUp,
    PlayerCompleteQuest,
    PlayerPickupGold,
    PlayerJump,
    PlayerLand,

    // Combat
    Slash,
    FireHit,
    IceHit,
    CritHit,
    BlockHit,

    // UI
    UIButton,
    UIConfirm,
    UICancel,
    UIPauseOpen,
    UIPauseClose,
    UIInventoryOpen,
    UIInventoryClose,
    UINewGame,
    UISaveGame,
    UIEquipItem,
    UIUnequipItem,
}

public enum MusicType {
    Menu,
    Exploration,
    Combat
}