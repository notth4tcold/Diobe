using TMPro;
using UnityEngine;

public class PlayerStatsUI : MonoBehaviour {
    [SerializeField] private Transform healthBar;
    [SerializeField] private Transform manaBar;
    [SerializeField] private TMP_Text nameText;

    private Player player;

    void Awake() {
        healthBar.localScale = new Vector3(0f, 1f, 1f);
        manaBar.localScale = new Vector3(0f, 1f, 1f);
        player = LevelManager.Instance.GetPlayer();
    }

    void OnEnable() {
        if (player == null) return;
        player.OnHealthPercentChanged += UpdateHealthDisplay;
        player.OnManaPercentChanged += UpdateManaDisplay;

        nameText.text = player.playerName;
        UpdateHealthDisplay(player.HealthPercent);
        UpdateManaDisplay(player.ManaPercent);
    }

    void OnDisable() {
        if (player == null) return;
        player.OnHealthPercentChanged -= UpdateHealthDisplay;
        player.OnManaPercentChanged -= UpdateManaDisplay;
    }

    private void UpdateHealthDisplay(float percent) {
        percent = Mathf.Clamp01(percent);
        healthBar.localScale = new Vector3(percent, 1f, 1f);
    }

    private void UpdateManaDisplay(float percent) {
        percent = Mathf.Clamp01(percent);
        manaBar.localScale = new Vector3(percent, 1f, 1f);
    }
}
