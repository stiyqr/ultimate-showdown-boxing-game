using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boxer : MonoBehaviour {


    public enum BoxerState {
        Dodge_L,
        Dodge_R,
        Guard,
        Dead,
        None,
    }


    public event EventHandler OnSuccessfulAttack;
    public event EventHandler OnBlockedAttack;
    public event EventHandler OnDodgedAttack;
    public event EventHandler<OnHPChangedEventArgs> OnHPChanged;
    public class OnHPChangedEventArgs : EventArgs {
        public float HPNormalized;
    }
    public event EventHandler OnBoxerDead;
    public class OnBoxerDeadEventArgs : EventArgs {
        public Boxer boxer;
    }


    [SerializeField] BoxerAnimator boxerAnimator;
    [SerializeField] Boxer opponent;
    [SerializeField] public int hp = 100;
    [SerializeField] public int dmg = 10;


    public BoxerState currentState;
    public bool isAlive;
    private int hpMax = 100;


    private void Awake() {
        currentState = BoxerState.None;
    }


    private void Update() {
        if (currentState == BoxerState.Dead) return;

        if (currentState != BoxerState.Dead && hp <= 0) {
            //Debug.Log(this + " DEAD");
            currentState = BoxerState.Dead;

            OnBoxerDead?.Invoke(this, new OnBoxerDeadEventArgs { boxer = this });
        }
    }


    public void PunchLeft() {
        if (!BoxingGameManager.Instance.IsGamePlaying() || currentState != BoxerState.None || BoxingGameManager.Instance.isGamePaused) return;

        boxerAnimator.AnimatePunch_L();

        if (opponent.currentState != BoxerState.Dodge_L) {
            DealDamage();
        }
        else {
            OnDodgedAttack?.Invoke(this, EventArgs.Empty);
        }
    }

    public void PunchRight() {
        if (!BoxingGameManager.Instance.IsGamePlaying() || currentState != BoxerState.None || BoxingGameManager.Instance.isGamePaused) return;

        boxerAnimator.AnimatePunch_R();

        if (opponent.currentState != BoxerState.Dodge_R) {
            DealDamage();
        }
        else {
            OnDodgedAttack?.Invoke(this, EventArgs.Empty);
        }
    }

    private void DealDamage() {
        if (opponent.currentState == BoxerState.Guard) {
            opponent.hp -= dmg / 2;
            OnBlockedAttack?.Invoke(this, EventArgs.Empty);
        }
        else {
            opponent.hp -= dmg;
            OnSuccessfulAttack?.Invoke(this, EventArgs.Empty);
        }

        opponent.OnHPChanged?.Invoke(this, new OnHPChangedEventArgs { HPNormalized = (float)opponent.hp / hpMax });
    }

    public BoxerState GetCurrentState() {
        return currentState;
    }
}
