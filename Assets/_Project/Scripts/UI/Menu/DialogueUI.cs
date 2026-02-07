using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {
    [Header("References")]
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] GameObject background;
    [SerializeField] Button okButton;

    private float textSpeed = 0.01f;

    string fullText;
    Coroutine typingCoroutine;
    bool isTyping;

    void Awake() {
        background.SetActive(false);
        gameObject.SetActive(false);
        okButton.interactable = false;
        okButton.onClick.AddListener(OnOkClicked);
    }

    public void Show(string text) {
        background.SetActive(true);
        gameObject.SetActive(true);

        fullText = text;
        dialogueText.text = "";
        okButton.interactable = false;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText() {
        isTyping = true;

        foreach (char c in fullText) {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        isTyping = false;
        okButton.interactable = true;
    }

    void OnOkClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        if (isTyping) {
            SkipTyping();
            return;
        }

        Close();
    }

    void SkipTyping() {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = fullText;
        isTyping = false;
        okButton.interactable = true;
    }

    void Close() {
        background.SetActive(false);
        gameObject.SetActive(false);
    }

    public bool IsOpened() {
        return gameObject.activeSelf && background.activeSelf;
    }
}
