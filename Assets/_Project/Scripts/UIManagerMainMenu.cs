using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerMainMenu : MonoBehaviour {

    public void OnNewGameClicked() {
        SceneManager.LoadScene("NewGame");
    }

    public void OnLoadGameClicked() {
        SceneManager.LoadScene("LoadGame");
    }

    public void OnOptionsClicked() {
        Debug.Log("Options!");
    }

    public void OnExitClicked() {
        Application.Quit();
    }
}
