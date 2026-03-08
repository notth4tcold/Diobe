using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Image image;

    private InventoryItem item;
    private IItemContainerUI container;
    private IItemContainerUI currentHoverContainer;
    private RectTransform rect;

    public InventoryItem Item => item;
    public RectTransform Rect => rect;

    private Player player;

    public void Init(InventoryItem item, IItemContainerUI container) {
        this.item = item;
        this.container = container;

        rect = GetComponent<RectTransform>();

        image.sprite = item.data.icon;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        ItemTooltipUI.Instance.Blocked = true;
        ItemTooltipUI.Instance.Hide();

        image.raycastTarget = false;
        transform.SetParent(UIManager.Instance.DragLayer);
        transform.SetAsLastSibling();

        container.OnBeginDrag(this, eventData);
    }

    public void OnDrag(PointerEventData eventData) {
        container.OnDrag(this, eventData);

        IItemContainerUI targetContainer = GetContainerUnderMouse(eventData);

        if (currentHoverContainer != targetContainer) {
            currentHoverContainer?.ClearVisualState(this);
            currentHoverContainer = targetContainer;
        }

        currentHoverContainer?.ShowVisualState(this, eventData);
    }

    public void OnEndDrag(PointerEventData eventData) {
        ItemTooltipUI.Instance.Blocked = false;
        image.raycastTarget = true;

        IItemContainerUI targetContainer = GetContainerUnderMouse(eventData);

        container.ClearVisualState(this);

        if (targetContainer == null) {
            container.RemoveItem(this);
            DropToWorld();
            return;
        }

        targetContainer.ClearVisualState(this);

        if (!targetContainer.CanReceiveItem(this, eventData)) {
            container.CancelDrag(this, eventData);
            return;
        }

        if (targetContainer == container) {
            targetContainer.ReceiveItem(this, eventData);
            return;
        }

        container.DetachItem(this);
        targetContainer.ReceiveItem(this, eventData);
        container = targetContainer;
    }

    private void DropToWorld() {
        AudioManager.Instance.PlaySFX(SFX.UIUnequipItem);
        player.DropItem(item);
    }

    private IItemContainerUI GetContainerUnderMouse(PointerEventData eventData) {
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results) {
            var link = result.gameObject.GetComponentInParent<ItemContainerLink>();
            if (link != null && link.Container != null) return link.Container;
        }

        return null;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        ItemTooltipUI.Instance.ShowUI(item.GetTooltip());
    }

    public void OnPointerExit(PointerEventData eventData) {
        ItemTooltipUI.Instance.Hide();
    }

    void OnEnable() {
        GameManager.Instance.SubscribeToPlayerReady(HandlePlayerReady);
        ItemTooltipUI.Instance.Hide();
    }

    void OnDisable() {
        GameManager.Instance.UnsubscribeFromPlayerReady(HandlePlayerReady);
        ItemTooltipUI.Instance.Hide();
    }

    private void HandlePlayerReady(Player p) {
        player = p;
    }
}
