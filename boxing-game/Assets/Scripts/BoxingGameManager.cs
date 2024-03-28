using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGameManager : MonoBehaviour {


    public static BoxingGameManager Instance { get; private set; }


    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;


    private enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        PlayerWin,
        PlayerLose,
    }


    [SerializeField] private Boxer playerBoxer;
    [SerializeField] private Boxer enemyBoxer;
    [SerializeField] private Connector connector;


    private State state;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    public bool isGamePaused = false;


    private void Awake() {
        Instance = this;

        state = State.WaitingToStart;
    }

    private void Start() {
        playerBoxer.OnBoxerDead += PlayerBoxer_OnBoxerDead;
        enemyBoxer.OnBoxerDead += EnemyBoxer_OnBoxerDead;
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;

        connector.OnKinectPauseAction += Connector_OnKinectPauseAction;
    }

    private void Connector_OnKinectPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void EnemyBoxer_OnBoxerDead(object sender, EventArgs e) {
        state = State.PlayerWin;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerBoxer_OnBoxerDead(object sender, EventArgs e) {
        state = State.PlayerLose;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Update() {
        switch (state) {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f) {
                    state  = State.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f) {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                break;
        }

        //Debug.Log(state);
    }


    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }

    public bool IsPlayerWin() {
        return state == State.PlayerWin;
    }

    public bool IsPlayerLose() {
        return state == State.PlayerLose;
    }

    public void TogglePauseGame() {
        if (state == State.PlayerWin || state == State.PlayerLose) return;

        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;

            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else {
            Time.timeScale = 1f;

            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }
}
