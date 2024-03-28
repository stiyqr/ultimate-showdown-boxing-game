using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;

public class Enemy : MonoBehaviour {


    private Boxer boxer;
    private float actionTimer = 0f;


    private void Awake() {
        boxer = GetComponent<Boxer>();
    }

    private void Start() {
        actionTimer = 0;
    }

    private void Update() {
        if (!BoxingGameManager.Instance.IsGamePlaying() || boxer.currentState == Boxer.BoxerState.Dead) return;

        actionTimer += Time.deltaTime;
        if (actionTimer >= 0.5f) {
            actionTimer = 0;
            GenerateRandomAction();
        }
    }


    private void GenerateRandomAction() {
        if (boxer.currentState != Boxer.BoxerState.None) {
            boxer.currentState = Boxer.BoxerState.None;
            return;
        }

        Random rnd = new Random();
        int randomNum = rnd.Next(0, 5);

        if (randomNum == 0) {
            boxer.PunchLeft();
        }
        else if (randomNum == 1) {
            boxer.PunchRight();
        }
        else if (randomNum == 2) {
            boxer.currentState = Boxer.BoxerState.Dodge_L;
        }
        else if (randomNum == 3) {
            boxer.currentState = Boxer.BoxerState.Dodge_R;
        }
        else {
            boxer.currentState = Boxer.BoxerState.Guard;
        }
    }
}
