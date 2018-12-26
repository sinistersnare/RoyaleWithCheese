using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    #region Singleton
    public static PlayerManager instance;

    private void Awake() {
        instance = this;
    }
    #endregion

    public Player bluePlayer;
    public Player redPlayer;
    /// <summary>
    /// false - This is a 1 player game.
    /// true - This is a 2 player game.
    /// </summary>
    public bool twoPlayerMode;
    public int scoreWinThreshold;
    
}
