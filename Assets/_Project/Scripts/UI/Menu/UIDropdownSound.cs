using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDropdownSound : MonoBehaviour, IPointerClickHandler {
    private TMP_Dropdown dropdown;

    void Start() {
        if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int index) {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
    }

    void OnDestroy() {
        if (dropdown != null) dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    public void OnPointerClick(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
    }
}
