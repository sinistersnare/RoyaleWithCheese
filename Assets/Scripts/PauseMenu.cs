using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
public class PauseMenu : MonoBehaviour {

    public static bool gameIsPaused = false;
    public GameObject pauseMenu;

    void Start() {
        gameIsPaused = false;
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) || XCI.GetButtonDown(XboxButton.Start, XboxController.Any)) {
            if (gameIsPaused) {
                this.ResumeGame();
            } else {
                this.PauseGame();
            }
        }
        if (gameIsPaused) {
            if (XCI.GetButtonDown(XboxButton.X, XboxController.Any)) {
                this.ResumeGame();
            }
            if (XCI.GetButtonDown(XboxButton.B, XboxController.Any)) {
                this.ToMainMenu();
            }
        }
	}
    
    public void ResumeGame() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        gameIsPaused = false;
    }
    private void PauseGame() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        gameIsPaused = true;
    }
    
    public void ToMainMenu() {
        Time.timeScale = 1;
        gameIsPaused = false;
        SceneManager.LoadScene(0);
    }
}
