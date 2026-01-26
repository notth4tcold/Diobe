using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerMainMenu : MonoBehaviour {
    public void OnNewGameClicked() {
        SceneManager.LoadScene("NewGame");
    }

    public void OnLoadGameClicked() {
        Debug.Log("Load Game!");
    }

    public void OnOptionsClicked() {
        Debug.Log("Options!");
    }

    public void OnExitClicked() {
        Application.Quit();
    }
}
