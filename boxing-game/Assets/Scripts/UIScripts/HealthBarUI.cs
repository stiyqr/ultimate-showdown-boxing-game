using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour {


    [SerializeField] private Boxer boxer;
    [SerializeField] private Image barImage;


    private void Awake() {
        barImage.fillAmount = 1f;
    }

    private void Start() {
        boxer.OnHPChanged += Boxer_OnHPChanged;
    }

    private void Boxer_OnHPChanged(object sender, Boxer.OnHPChangedEventArgs e) {
        barImage.fillAmount = e.HPNormalized;

        if (e.HPNormalized <= 0.2f) {
            barImage.color = Color.red;
        }
        else if (e.HPNormalized <= 0.4f) {
            barImage.color = Color.yellow;
        }
    }
}
