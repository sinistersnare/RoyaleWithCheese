using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class MainMenuManager : MonoBehaviour {
    public GameObject loadingText;
    public GameObject navHolder;
    public GameObject creditHolder;

    private bool creditsActive = false;
    
    private void Update() {
        if (XCI.GetButtonDown(XboxButton.A, XboxController.First)) {
            if (!this.creditsActive) {
                this.ToggleCredits();
                this.creditsActive = true;
            }
        }
        if (XCI.GetButtonDown(XboxButton.B, XboxController.First)) {
            if (!this.creditsActive) {
                this.To2PlayerBattle();
            } else {
                this.creditsActive = false;
                this.ToggleCredits();
            }
        }
        if (XCI.GetButtonDown(XboxButton.X, XboxController.First)) {
            if (!this.creditsActive) {
                this.To1PlayerChallenge();
            }
        }
        if (XCI.GetButtonDown(XboxButton.Y, XboxController.First)) {
            if (!this.creditsActive) {
                this._QuitGame();
            }
        }
    }

    public void ToMainMenu() {
        _ToMainMenu();
    }
    void _ToMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void To2PlayerBattle() {
        this.loadingText.SetActive(true);
        Invoke("_To2PlayerBattle", 0f);
    }
    void _To2PlayerBattle() {
        SceneManager.LoadScene(1);
    }

    public void To1PlayerChallenge() {
        this.loadingText.SetActive(true);
        Invoke("_To1PlayerChallenge", 0f);
    }
    void _To1PlayerChallenge() {
        SceneManager.LoadScene(2);
    }

    public void ToggleCredits() {
        this.navHolder.SetActive(!this.navHolder.activeInHierarchy);
        this.creditHolder.SetActive(!this.creditHolder.activeInHierarchy);
    }

    public void RestartScene() {
        Invoke("_RestartScene", 2f);
    }

    void _RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame() {
        Invoke("_QuitGame", 1f);
    }
    void _QuitGame() {
        Debug.Log("OverlayScreenHandler#QuitGame called.");
        Application.Quit();
    }
}
