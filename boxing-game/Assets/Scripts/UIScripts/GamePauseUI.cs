using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour {


    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button continueButton;


    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        continueButton.onClick.AddListener(() => {
            BoxingGameManager.Instance.TogglePauseGame();
        });
    }

    private void Start() {
        BoxingGameManager.Instance.OnGamePaused += BoxingGameManager_OnGamePaused;
        BoxingGameManager.Instance.OnGameUnpaused += BoxingGameManager_OnGameUnpaused;

        Hide();
    }

    private void BoxingGameManager_OnGameUnpaused(object sender, System.EventArgs e) {
        Hide();
    }

    private void BoxingGameManager_OnGamePaused(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
