using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverWindow : MonoBehaviour {

    private static GameOverWindow instance;

    public static GameOverWindow GetInstance() {
        return instance;
    }

    private void Awake() {
        gameObject.SetActive(true);
    }

    private void Start() {
        Level.GetInstance().OnGameOver += Level_OnGameOver;
    }

    private void Level_OnGameOver(object sender, System.EventArgs e) {
        int score = Level.GetInstance().GetScore();
        Debug.Log(score);
        Debug.Log("ON GAME OVER!");
    }
}
