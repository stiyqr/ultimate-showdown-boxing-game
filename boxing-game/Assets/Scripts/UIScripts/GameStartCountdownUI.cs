using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class GameStartCountdownUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI countdownText;


    private void Start () {
        BoxingGameManager.Instance.OnStateChanged += BoxingGameManager_OnStateChanged;

        Hide();
    }

    private void Update() {
        countdownText.text = Mathf.Ceil(BoxingGameManager.Instance.GetCountdownToStartTimer()).ToString();
    }


    private void BoxingGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (BoxingGameManager.Instance.IsCountdownToStartActive()) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
