
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerMainMenu : MonoBehaviour {

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject optionsMenuUI;

    void Awake() {
        AudioManager.Instance.PlayMusic(MusicType.Menu);
        mainMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
    }

    public void OnNewGameClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        SceneManager.LoadScene("NewGame");
    }

    public void OnLoadGameClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        SceneManager.LoadScene("LoadGame");
    }

    public void OnOptionsClicked() {
        AudioManager.Instance.PlaySFX(SFX.UIButton);
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    public void OnOptionsCloseClicked() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        mainMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
    }

    public void OnExitClicked() {
        AudioManager.Instance.PlaySFX(SFX.UICancel);
        Application.Quit();
    }
}
