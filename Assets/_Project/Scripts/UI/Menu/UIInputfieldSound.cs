using UnityEngine;
using UnityEngine.EventSystems;

public class UIInputfieldSound : MonoBehaviour, ISelectHandler {
    public void OnSelect(BaseEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
    }
}
