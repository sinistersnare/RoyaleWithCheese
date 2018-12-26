using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AdventureGuiController : MonoBehaviour {

    public Text timeText;
    public Text itemsHeldText;
    public Text winText;

    private float timeAlive;
	void Update () {
        if (PlayerManager.instance.bluePlayer == null) return;
        int playerScore = PlayerManager.instance.bluePlayer.Score;
        int winThreshold = PlayerManager.instance.scoreWinThreshold;
        if (playerScore == PlayerManager.instance.scoreWinThreshold) {
            this.timeText.gameObject.SetActive(false);
            this.itemsHeldText.gameObject.SetActive(false);
            this.winText.gameObject.SetActive(true);
            this.winText.text = "You won! In " + this.timeAlive + " seconds! Try to beat it!\nGoing to main menu...";
            this.ToMainMenu();
        } else {
            this.timeAlive += Time.deltaTime;
            this.timeText.text = "Time Passed: " + this.timeAlive;
            this.itemsHeldText.text = "Items Held: " + playerScore + " / " + winThreshold;
        }
    }
    private void ToMainMenu() {
        Invoke("_ToMainMenu", 2f);
    }

    private void _ToMainMenu() {
        SceneManager.LoadScene(0);
    }
}
