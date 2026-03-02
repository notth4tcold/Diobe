using System;
using TMPro;
using UnityEngine;

public class PlayerAttributesUI : MonoBehaviour {
    [SerializeField] private TMP_Text nameText;

    [SerializeField] private TMP_Text levelText;

    [SerializeField] private TMP_Text strenghtText;
    [SerializeField] private TMP_Text dexterityText;
    [SerializeField] private TMP_Text vitalityText;
    [SerializeField] private TMP_Text intelligenceText;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text armorText;

    private Player player;

    void Initialize() {
        // TODO Adicionar eventos para atualizar esses valores

        nameText.text = player.playerName;

        levelText.text = player.level.ToString();

        strenghtText.text = player.stats.strength.ToString();
        dexterityText.text = player.stats.dexterity.ToString();
        vitalityText.text = player.stats.vitality.ToString();
        intelligenceText.text = player.stats.intelligence.ToString();

        healthText.text = player.resources.health.ToString();
        manaText.text = player.resources.mana.ToString();
        attackText.text = player.combat.attack.ToString();
        armorText.text = player.combat.armor.ToString();
    }

    void OnEnable() {
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
    }

    void OnDisable() {
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
    }

    private void HandlePlayerReady(Player p) {
        if (player != null) return;
        player = p;
        Initialize();
    }
}
