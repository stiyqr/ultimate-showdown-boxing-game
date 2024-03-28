using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {


    [SerializeField] private Image playerWinPNG;
    [SerializeField] private Image playerWinConfetti;
    [SerializeField] private Image playerLosePNG;
    [SerializeField] private Image playerLoseFace;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button playAgainButton;


    private void Awake() {
        homeButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        playAgainButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
    }

    private void Start() {
        BoxingGameManager.Instance.OnStateChanged += BoxingGameManager_OnStateChanged;

        Hide();
    }


    private void BoxingGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (BoxingGameManager.Instance.IsPlayerWin()) {
            ShowPlayerWin();
        }
        else if (BoxingGameManager.Instance.IsPlayerLose()) {
            ShowPlayerLose();
        }
        else {
            Hide();
        }
    }

    private void ShowPlayerWin() {
        gameObject.SetActive(true);
        playerLosePNG.gameObject.SetActive(false);
        playerLoseFace.gameObject.SetActive(false);
        //playAgainButton.Select();
    }

    private void ShowPlayerLose() {
        gameObject.SetActive(true);
        playerWinPNG.gameObject.SetActive(false);
        playerWinConfetti.gameObject.SetActive(false);
        //playAgainButton.Select();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
