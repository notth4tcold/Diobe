using UnityEngine;
using UnityEngine.EventSystems;

public class SliderSoundUI : MonoBehaviour, IPointerUpHandler {

    public void OnPointerUp(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
    }
}
