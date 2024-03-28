using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {


    public static GameInput Instance { get; private set; }


    public event EventHandler OnPlayerPunch_L;
    public event EventHandler OnPlayerPunch_R;
    public event EventHandler<OnPlayerDodge_LEventArgs> OnPlayerDodge_L;
    public class OnPlayerDodge_LEventArgs : EventArgs {
        public bool isDodging_L;
    }
    public event EventHandler<OnPlayerDodge_REventArgs> OnPlayerDodge_R;
    public class OnPlayerDodge_REventArgs : EventArgs {
        public bool isDodging_R;
    }
    public event EventHandler<OnPlayerGuardEventArgs> OnPlayerGuard;
    public class OnPlayerGuardEventArgs : EventArgs {
        public bool isGuard;
    }
    public event EventHandler OnPauseAction;

    private PlayerInputActions playerInputActions;


    private void Awake() {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Punch_L.performed += Punch_L_performed;
        playerInputActions.Player.Punch_R.performed += Punch_R_performed;

        playerInputActions.Player.Dodge_L.performed += Dodge_L_performed;
        playerInputActions.Player.Dodge_L.canceled += Dodge_L_canceled;
        playerInputActions.Player.Dodge_R.performed += Dodge_R_performed;
        playerInputActions.Player.Dodge_R.canceled += Dodge_R_canceled;

        playerInputActions.Player.Guard.performed += Guard_performed;
        playerInputActions.Player.Guard.canceled += Guard_canceled;

        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy() {
        playerInputActions.Player.Punch_L.performed -= Punch_L_performed;
        playerInputActions.Player.Punch_R.performed -= Punch_R_performed;

        playerInputActions.Player.Dodge_L.performed -= Dodge_L_performed;
        playerInputActions.Player.Dodge_L.canceled -= Dodge_L_canceled;
        playerInputActions.Player.Dodge_R.performed -= Dodge_R_performed;
        playerInputActions.Player.Dodge_R.canceled -= Dodge_R_canceled;

        playerInputActions.Player.Guard.performed -= Guard_performed;
        playerInputActions.Player.Guard.canceled -= Guard_canceled;

        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Guard_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerGuard?.Invoke(this, new OnPlayerGuardEventArgs { isGuard = false });
    }

    private void Guard_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerGuard?.Invoke(this, new OnPlayerGuardEventArgs { isGuard = true });
    }

    private void Dodge_R_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerDodge_R?.Invoke(this, new OnPlayerDodge_REventArgs { isDodging_R = false });
    }

    private void Dodge_R_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerDodge_R?.Invoke(this, new OnPlayerDodge_REventArgs { isDodging_R = true });
    }

    private void Dodge_L_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerDodge_L?.Invoke(this, new OnPlayerDodge_LEventArgs { isDodging_L = false });
    }

    private void Dodge_L_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerDodge_L?.Invoke(this, new OnPlayerDodge_LEventArgs { isDodging_L = true });
    }

    private void Punch_R_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerPunch_R?.Invoke(this, EventArgs.Empty);
    }

    private void Punch_L_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPlayerPunch_L?.Invoke(this, EventArgs.Empty);
    }

}
