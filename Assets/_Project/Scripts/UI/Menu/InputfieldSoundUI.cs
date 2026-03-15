using UnityEngine;
using UnityEngine.EventSystems;

public class InputfieldSoundUI : MonoBehaviour, ISelectHandler {
    public void OnSelect(BaseEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
    }
}
