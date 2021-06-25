using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level : MonoBehaviour
{
    private static Level instance;

    // Time interval between object instatiation
    public const float RewardInterval = 1f;
    public const float BlackHoleInterval = 5f;
    public const float AsteroidInterval = 3f;

    // Timers
    private float rewardTimer = RewardInterval;
    private float blackHoleTimer = BlackHoleInterval;
    private float asteroidTimer = AsteroidInterval;
    private float speedUpTimer = 0f;

    // Rocket & UI
    private Capsule rocket;
    private GameObject live1, live2, live3;
    private TextMeshProUGUI scoreText;
    private ScrollingBackground background;

    // Level state
    private State state;
    private enum State {
        WaitingToStart,
        Playing,
        Crashed,
    }

    // Auxiliary 
    private int score = 0;
    public int lives = 3;
    public event EventHandler OnGameOver;
    public GameObject bigExplosion;

    public static Level GetInstance() {
        return instance;
    }

    private void Awake() {
        instance = this;

        rocket = Capsule.GetInstance();
        live1 = GameObject.FindGameObjectWithTag("Live1");
        live2 = GameObject.FindGameObjectWithTag("Live2");
        live3 = GameObject.FindGameObjectWithTag("Live3");
        scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<TextMeshProUGUI>();
        background = GameObject.FindGameObjectWithTag("Background").GetComponent<ScrollingBackground>();

        rewardTimer = 2f;
        blackHoleTimer = 5f;

        state = State.WaitingToStart;

        Capsule.GetInstance().OnStartedPlaying += Rocket_OnStartedPlaying;
        Capsule.GetInstance().OnAsteroidHit += Rocket_OnAsteroidHit;
        Capsule.GetInstance().OnRewardHit += Rocket_OnRewardHit;
        Capsule.GetInstance().OnBlackHoleExit += Rocket_OnBlackHoleExit;

        lives = 3;
    }

    private void Start() {
        // Listen rocket state:
        // Capsule.GetInstance().OnStartedPlaying += Rocket_OnStartedPlaying;
        // Capsule.GetInstance().OnAsteroidHit += Rocket_OnAsteroidHit;
        // Capsule.GetInstance().OnRewardHit += Rocket_OnRewardHit;
        // Capsule.GetInstance().OnBlackHoleExit += Rocket_OnBlackHoleExit;
        
        // lives = 3;
    }

    private void Update() {

        if (state != State.Playing) {
            return;
        }

        speedUpTimer = Math.Max(0, speedUpTimer - Time.deltaTime);
        if (speedUpTimer > 0) {
            background.SetSpeed(20f);
            blackHoleTimer = BlackHoleInterval;
            asteroidTimer = AsteroidInterval;
            rewardTimer = 0.01f;
        } else {
            background.SetSpeed(0.1f);
        }

        // spawn rewards
        rewardTimer -= Time.deltaTime;
        if(rewardTimer < 0) {
            rewardTimer += RewardInterval;
            var reward = Instantiate(Assets.GetInstance().pfReward).GetComponent<Reward>();
            if(speedUpTimer > 0){
                reward.SetSpeed(20f);
            }
        }

        // spawn obstacles
        System.Random rnd = new System.Random();
        if (rnd.Next(10) <= 2) {
            blackHoleTimer -= Time.deltaTime;
            if(blackHoleTimer < 0) {
                blackHoleTimer += BlackHoleInterval;
                var blackhole = Instantiate(Assets.GetInstance().pfBlackHole).GetComponent<BlackHole>();
                if(speedUpTimer > 0) {
                    blackhole.SetSpeed(20f);
                }
            }
        } else {
            asteroidTimer -= Time.deltaTime;
            if(asteroidTimer < 0) {
                asteroidTimer += AsteroidInterval;
                var asteroid = Instantiate(Assets.GetInstance().pfAsteroid).GetComponent<Asteroid>();
                if(speedUpTimer > 0) {
                    asteroid.SetSpeed(20f);
                }
            }
        }
    }


    private void Rocket_OnStartedPlaying(object sender, System.EventArgs e) {
        live1.gameObject.SetActive(true);
        live2.gameObject.SetActive(true);
        live3.gameObject.SetActive(true);
        score = 0;
        lives = 3;
        scoreText.text = score.ToString();
        rewardTimer = RewardInterval;
        blackHoleTimer = BlackHoleInterval;
        asteroidTimer = AsteroidInterval;
        
        state = State.Playing;
    }

    private void Rocket_OnRewardHit(object sender, System.EventArgs e) {
        score += 1;
        scoreText.text = score.ToString();
    }

    private void Rocket_OnAsteroidHit(object sender, System.EventArgs e) {
        lives = Math.Max(0, lives - 1);
        switch(lives){
            default: break;
            case 2: {
                live1.gameObject.SetActive(false);
                break;
            }
            case 1: {
                live2.gameObject.SetActive(false);
                break;
            }
            case 0: {
                live3.gameObject.SetActive(false);
                
                GameObject crashExplosion = Instantiate(bigExplosion, transform.position, transform.rotation);
                Destroy(crashExplosion, 1.2f);

                speedUpTimer = 0f;

                state = State.Crashed;
                if (OnGameOver != null) OnGameOver(this, EventArgs.Empty);
                break;
            }
        }
    }

    private void Rocket_OnBlackHoleExit(object sender, System.EventArgs e) {
        speedUpTimer += 3f;
        // Debug.Log("Speed up - start");
    }



    public int GetScore() {
        return score;
    }
}
