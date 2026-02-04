using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour {
    public Slider master;
    public Slider sfx;
    public Slider ui;
    public Slider music;

    void Start() {
        master.value = AudioManager.Instance.masterVolume;
        sfx.value = AudioManager.Instance.sfxVolume;
        ui.value = AudioManager.Instance.uiVolume;
        music.value = AudioManager.Instance.musicVolume;
    }

    public void SetMaster(float v) {
        AudioManager.Instance.masterVolume = v;
        AudioManager.Instance.UpdateVolumes();
    }

    public void SetSFX(float v) {
        AudioManager.Instance.sfxVolume = v;
        AudioManager.Instance.UpdateVolumes();
    }

    public void SetUI(float v) {
        AudioManager.Instance.uiVolume = v;
        AudioManager.Instance.UpdateVolumes();
    }

    public void SetMusic(float v) {
        AudioManager.Instance.musicVolume = v;
        AudioManager.Instance.UpdateVolumes();
    }
}
