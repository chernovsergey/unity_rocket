using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Capsule : MonoBehaviour
{
    // Behaviour related constans
    public float MAX_ANGLE = 45f;
    public float ROTATION_SPEED = 20f;
    public float ROATATION_DECREASE_SPEED = 1000f;
    public float VELOCITY_MULTIPLIER = 20f;
    public float MAX_X_POSITION = 24;
    public float MIN_X_POSITION = -24;
    public float MAX_Y_POSITION = 43;
    public float MIN_Y_POSITION = -43;


    // Auxiliary variables
    public GameObject bumpEffect;
    private Rigidbody2D rigitBody;
    private static Capsule instance;
    private bool moveLeft, moveRight, moveUp, moveDown;


    // Events for subscibers
    public event EventHandler OnStartedPlaying;
    public event EventHandler OnAsteroidHit;
    public event EventHandler OnRewardHit;
    public event EventHandler OnBlackHoleExit;


    // Subscribed events
    public event EventHandler OnGameOver;

    // UI 
    Button startButton;
    Button restartButton;
    TextMeshProUGUI startTitle;

    private bool startButtonClicked;
    private bool restartButtonClicked;


    // Object states
    private State state;
    private enum State {
        WaitingToStart,
        Playing
    }

    public static Capsule GetInstance() {
        return instance;
    }

    void Awake() 
    {
        instance = this;
        state = State.WaitingToStart;
    }

    void Start () {
        rigitBody = GetComponent<Rigidbody2D>();
        rigitBody.bodyType = RigidbodyType2D.Static;

        // not sure this is the right place to react on UI input
        GameObject startTitleObj = GameObject.FindWithTag("MainTitle");
        startTitle = startTitleObj.GetComponent<TextMeshProUGUI>();

        GameObject startButtonObj = GameObject.FindWithTag("StartButton");
        startButton = startButtonObj.GetComponent<Button>();
        startButton.onClick.AddListener(StartGame);
        startButtonClicked = false;

        GameObject restartButtonObj = GameObject.FindWithTag("RestartButton");
        restartButton = restartButtonObj.GetComponent<Button>();
        restartButton.onClick.AddListener(ResetGame);
        restartButtonClicked = false;

        Level.GetInstance().OnGameOver += Level_OnGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
        case State.WaitingToStart:
            if (startButtonClicked || restartButtonClicked) {
                state = State.Playing;
                rigitBody.bodyType = RigidbodyType2D.Dynamic;
                if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
            }
            break;
        case State.Playing:
            {
                moveLeft = Input.GetKey(KeyCode.LeftArrow);
                moveRight = Input.GetKey(KeyCode.RightArrow);
                moveUp = Input.GetKey(KeyCode.UpArrow); 
                moveDown = Input.GetKey(KeyCode.DownArrow);
            }
            break;
        }
    }

    void StartGame() {
        startButtonClicked = true;

        // hide all menu elements
        startButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        startTitle.gameObject.SetActive(false);
    }

    void ResetGame(){
        Debug.Log("RestartClicked");
        restartButtonClicked = true;

        // reset rocket position
        gameObject.SetActive(true);
        transform.position = new Vector3(0,0,0);
        rigitBody.velocity = new Vector2(0,0);
        transform.rotation = new Quaternion(0,0,0,0);
        rigitBody.bodyType = RigidbodyType2D.Dynamic;

        // hide buttons
        startButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        startTitle.gameObject.SetActive(false);

        // reset lives
        GameObject.FindGameObjectWithTag("Live1").gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Live2").gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Live3").gameObject.SetActive(true);

        // reset score counter
        GameObject textMesh = GameObject.FindGameObjectWithTag("ScoreText");
        TextMeshProUGUI scoreText = textMesh.GetComponent<TextMeshProUGUI>();
        scoreText.text  = "0";

        // state = State.Playing;
        // if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
    }

    private void Level_OnGameOver(object sender, System.EventArgs e) {
        startButtonClicked = false;
        restartButtonClicked = false;

        // hide rocket
        gameObject.SetActive(false);
        rigitBody.bodyType = RigidbodyType2D.Static;

        // reset all except start button
        restartButton.gameObject.SetActive(true);
        startTitle.gameObject.SetActive(true);

        state = State.WaitingToStart;
    }
 
    void FixedUpdate() {
        if (state != State.Playing) {
            return;
        }

        Vector2 vup = new Vector2();
        Vector2 vleft = new Vector2();
        Vector2 vright = new Vector2();
        Vector2 vdown = new Vector2();

        if(moveUp) {
            vup = Vector2.up;
        }
        if(moveDown) {
            vdown = Vector2.down;
        }
        if(moveLeft){
            vleft = Vector2.left;
        }
        if(moveRight){
            vright = Vector2.right;
        }
        if(moveUp || moveDown || moveRight || moveLeft){
            rigitBody.velocity = (vup + vleft + vright + vdown) * VELOCITY_MULTIPLIER;

            if(Mathf.Abs(transform.rotation.z) <= MAX_ANGLE ){
                transform.eulerAngles = new Vector3(0, 0,  - rigitBody.velocity.x * ROTATION_SPEED * Time.deltaTime);
            }
        }   

        // bound positions
        transform.position = new Vector3(
            Mathf.Max(Mathf.Min(transform.position.x, MAX_X_POSITION), MIN_X_POSITION),
            Mathf.Max(Mathf.Min(transform.position.y, MAX_Y_POSITION), MIN_Y_POSITION),
            transform.position.z
        );

        // // decrease angle
        float deltaAngle = (0.0f - transform.rotation.z);
        transform.Rotate(new Vector3(0, 0, deltaAngle * Time.deltaTime * ROATATION_DECREASE_SPEED));

        // // decrease velocity
        rigitBody.velocity = new Vector2(
            rigitBody.velocity.x + (0.0f - rigitBody.velocity.x) * Time.deltaTime, 
            rigitBody.velocity.y + (0.0f - rigitBody.velocity.y) * Time.deltaTime
        ); 
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        switch(other.gameObject.tag) {
            case ("Asteroid"): {
                GameObject bumpExplosion = Instantiate(bumpEffect, transform.position, transform.rotation);
                Destroy(bumpExplosion, 2f);
                if (OnAsteroidHit != null) OnAsteroidHit(this, EventArgs.Empty);
                break;
            }
            case ("Reward"): {
                if (OnRewardHit != null) OnRewardHit(this, EventArgs.Empty);
                break;
            }
            case ("BlackHole"): {
                if (OnBlackHoleExit != null) OnBlackHoleExit(this, EventArgs.Empty);
                break;
            }
        }
    }
}
