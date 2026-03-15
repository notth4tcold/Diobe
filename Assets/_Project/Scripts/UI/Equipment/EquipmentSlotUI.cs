using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour {
    [SerializeField] private Image background;
    [SerializeField] private EquipmentType slotType;
    [SerializeField] private RingSlot ringSlot;

    public EquipmentType SlotType => slotType;
    public RingSlot RingSlot => ringSlot;

    public void SetHighlight(Color color) {
        background.color = color;
    }

    public void ClearHighlight() {
        background.color = Color.white;
    }
}