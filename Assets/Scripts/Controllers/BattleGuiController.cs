using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleGuiController : MonoBehaviour {

    public Text blueScoreText;
    public Text redScoreText;
    public Text winText;

    public Slider blueHealthBar;
    public Slider redHealthBar;

    public Slider blueGunSlider;
    public Slider redGunSlider;

    public Slider blueBombSlider;
    public Slider redBombSlider;

    public Slider bluePodSlider;
    public Slider redPodSlider;

    private Player bluePlayer;
    private Player redPlayer;
    
    private string winTextTemplate = " Player Wins!\nGoing back to menu...";

    void Update () {
        if (this.bluePlayer == null || this.redPlayer == null) {
            this.bluePlayer = PlayerManager.instance.bluePlayer;
            this.redPlayer = PlayerManager.instance.redPlayer;
        }

        int scoreThreshold = PlayerManager.instance.scoreWinThreshold;
        int blueScore = this.bluePlayer.Score;
        int redScore = this.redPlayer.Score;

        this.blueScoreText.text = "Blue Score: " + blueScore + " / " + scoreThreshold;
        this.redScoreText.text = "Red Score: " + redScore + " / " + scoreThreshold;

        if (blueScore >= scoreThreshold) {
            this.winText.gameObject.SetActive(true);
            this.winText.text = "Blue" + winTextTemplate;
            this.ToMainMenu();
            return;
        } else if (redScore >= scoreThreshold) {
            this.winText.gameObject.SetActive(true);
            this.winText.text = "Red" + winTextTemplate;
            this.ToMainMenu();
            return;
        }

        this.blueHealthBar.value = this.bluePlayer.Health;
        this.redHealthBar.value =  this.redPlayer.Health;

        if (this.bluePlayer.CurrentGun == null) {
            this.blueGunSlider.value = 0;
        } else {
            this.blueGunSlider.value = this.bluePlayer.CurrentGun.GunValue;
        }
        if (this.redPlayer.CurrentGun == null) {
            this.redGunSlider.value = 0;
        } else {
            this.redGunSlider.value = this.redPlayer.CurrentGun.GunValue;
        }

        this.blueBombSlider.value = this.bluePlayer.BombValue;
        this.redBombSlider.value = this.redPlayer.BombValue;

        this.bluePodSlider.value = this.bluePlayer.PodValue;
        this.redPodSlider.value = this.redPlayer.PodValue;
	}

    private void ToMainMenu() {
        Invoke("_ToMainMenu", 2f);
    }

    private void _ToMainMenu() {
        SceneManager.LoadScene(0);
    }

}
