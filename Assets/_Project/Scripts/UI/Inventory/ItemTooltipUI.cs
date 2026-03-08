using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour {
    public static ItemTooltipUI Instance;

    [SerializeField] GameObject root;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform backgroundRect;

    private Transform worldTarget;
    private bool followMouse;
    private Vector2 tooltipSize;

    public bool Blocked { get; set; }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        canvasGroup.blocksRaycasts = false;
        Hide();
    }

    void Update() {
        if (!root.activeSelf) return;

        Vector2 pos;

        if (worldTarget != null) {
            pos = Camera.main.WorldToScreenPoint(worldTarget.position + Vector3.up * 1.5f);
        } else if (followMouse) {
            pos = Mouse.current.position.ReadValue() + new Vector2(96, -96);
        } else {
            return;
        }

        if (pos.y < tooltipSize.y * 0.5f) pos += new Vector2(0, tooltipSize.y + 32);
        transform.position = pos;
    }

    public void ShowWorld(string content, Transform target) {
        root.SetActive(true);
        text.text = content;

        worldTarget = target;
        followMouse = false;

        GetTooltipSize();
    }

    public void ShowUI(string content) {
        if (Blocked) return;

        root.SetActive(true);
        text.text = content;

        worldTarget = null;
        followMouse = true;

        GetTooltipSize();
    }

    void GetTooltipSize() {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRect);
        tooltipSize = backgroundRect.rect.size;
    }

    public void Hide() {
        root.SetActive(false);
        worldTarget = null;
        followMouse = false;
    }

    public bool IsVisible => root.activeSelf;
}