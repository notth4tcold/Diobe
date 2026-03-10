using System;
using TMPro;
using UnityEngine;

public class PlayerAttributesUI : MonoBehaviour {
    [SerializeField] private TMP_Text nameText;

    [SerializeField] private TMP_Text levelText;

    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text dexterityText;
    [SerializeField] private TMP_Text vitalityText;
    [SerializeField] private TMP_Text intelligenceText;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text armorText;

    private Player player;

    void Initialize() {
        UpdateUI();

        player.stats.OnStatsChanged += UpdateUI;
    }

    void UpdateUI() {
        if (player == null) return;

        nameText.text = player.playerName;
        levelText.text = player.level.ToString();

        strengthText.text = player.stats.Get(StatType.Strength).ToString();
        dexterityText.text = player.stats.Get(StatType.Dexterity).ToString();
        vitalityText.text = player.stats.Get(StatType.Vitality).ToString();
        intelligenceText.text = player.stats.Get(StatType.Intelligence).ToString();

        healthText.text = player.resources.MaxHealth.ToString();
        manaText.text = player.resources.MaxMana.ToString();
        attackText.text = player.combat.Damage.ToString();
        armorText.text = player.combat.Armor.ToString();
    }

    void OnEnable() {
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
        if (player != null) Initialize();
    }

    void OnDisable() {
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
        if (player != null) player.stats.OnStatsChanged -= UpdateUI;
    }

    private void HandlePlayerReady(Player p) {
        if (player != null) return;
        player = p;
        Initialize();
    }
}
