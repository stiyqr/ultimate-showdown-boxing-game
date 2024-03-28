using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    [SerializeField] private GameInput gameInput;
    [SerializeField] private Connector connector;
    
    
    private Boxer boxer;


    private void Awake() {
        boxer = GetComponent<Boxer>();
    }

    private void Start() {
        gameInput.OnPlayerPunch_L += GameInput_OnPlayerPunch_L;
        gameInput.OnPlayerPunch_R += GameInput_OnPlayerPunch_R;
        gameInput.OnPlayerDodge_L += GameInput_OnPlayerDodge_L;
        gameInput.OnPlayerDodge_R += GameInput_OnPlayerDodge_R;
        gameInput.OnPlayerGuard += GameInput_OnPlayerGuard;

        connector.OnKinectPunch_L += Connector_OnKinectPunch_L;
        connector.OnKinectPunch_R += Connector_OnKinectPunch_R;
        connector.OnKinectDodge_L += Connector_OnKinectDodge_L;
        connector.OnKinectDodge_R += Connector_OnKinectDodge_R;
        connector.OnKinectGuard += Connector_OnKinectGuard;
        connector.OnKinectIdle += Connector_OnKinectIdle;
    }

    private void Connector_OnKinectIdle(object sender, System.EventArgs e) {
        if (boxer.currentState == Boxer.BoxerState.Dead) return;

        boxer.currentState = Boxer.BoxerState.None;
    }

    private void Connector_OnKinectGuard(object sender, Connector.OnKinectGuardEventArgs e) {
        if (boxer.currentState == Boxer.BoxerState.Dead) return;

        if (e.isGuard) {
            boxer.currentState = Boxer.BoxerState.Guard;
        }
        else {
            boxer.currentState = Boxer.BoxerState.None;
        }
    }

    private void Connector_OnKinectDodge_R(object sender, Connector.OnKinectDodge_REventArgs e) {
        if (boxer.currentState == Boxer.BoxerState.Dead) return;

        if (e.isDodging_R) {
            boxer.currentState = Boxer.BoxerState.Dodge_R;
        }
        else {
            boxer.currentState = Boxer.BoxerState.None;
        }
    }

    private void Connector_OnKinectDodge_L(object sender, Connector.OnKinectDodge_LEventArgs e) {
        if (boxer.currentState == Boxer.BoxerState.Dead) return;

        if (e.isDodging_L) {
            boxer.currentState = Boxer.BoxerState.Dodge_L;
        }
        else {
            boxer.currentState = Boxer.BoxerState.None;
        }
    }

    private void Connector_OnKinectPunch_R(object sender, System.EventArgs e) {
        boxer.PunchRight();
    }

    private void Connector_OnKinectPunch_L(object sender, System.EventArgs e) {
        boxer.PunchLeft();
    }

    private void GameInput_OnPlayerGuard(object sender, GameInput.OnPlayerGuardEventArgs e) {
        if (boxer.currentState == Boxer.BoxerState.Dead) return;

        if (e.isGuard) {
            boxer.currentState = Boxer.BoxerState.Guard;
        }
        else {
            boxer.currentState = Boxer.BoxerState.None;
        }
    }

    private void GameInput_OnPlayerDodge_R(object sender, GameInput.OnPlayerDodge_REventArgs e) {
        if (boxer.currentState == Boxer.BoxerState.Dead) return;

        if (e.isDodging_R) {
            boxer.currentState = Boxer.BoxerState.Dodge_R;
        }
        else {
            boxer.currentState = Boxer.BoxerState.None;
        }
    }

    private void GameInput_OnPlayerDodge_L(object sender, GameInput.OnPlayerDodge_LEventArgs e) {
        if (boxer.currentState == Boxer.BoxerState.Dead) return;

        if (e.isDodging_L) {
            boxer.currentState = Boxer.BoxerState.Dodge_L;
        }
        else {
            boxer.currentState = Boxer.BoxerState.None;
        }
    }


    private void GameInput_OnPlayerPunch_L(object sender, System.EventArgs e) {
        boxer.PunchLeft();
    }
    private void GameInput_OnPlayerPunch_R(object sender, System.EventArgs e) {
        boxer.PunchRight();
    }
}
