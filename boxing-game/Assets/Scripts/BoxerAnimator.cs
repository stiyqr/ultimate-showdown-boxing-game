using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerAnimator : MonoBehaviour {


    private const string IS_DODGING_L = "IsDodging_L";
    private const string IS_DODGING_R = "IsDodging_R";
    private const string PUNCH_L = "Punch_L";
    private const string PUNCH_R = "Punch_R";
    private const string IS_GUARD = "IsGuard";
    private const string IS_DEAD = "IsDead";


    [SerializeField] private Boxer boxer;


    private Animator animator;


    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        animator.SetBool(IS_DODGING_L, boxer.currentState == Boxer.BoxerState.Dodge_L);
        animator.SetBool(IS_DODGING_R, boxer.currentState == Boxer.BoxerState.Dodge_R);
        animator.SetBool(IS_GUARD, boxer.currentState == Boxer.BoxerState.Guard);
        animator.SetBool(IS_DEAD, boxer.currentState == Boxer.BoxerState.Dead);
    }

    public void AnimatePunch_L() {
        animator.SetTrigger(PUNCH_L);
    }

    public void AnimatePunch_R() {
        animator.SetTrigger(PUNCH_R);
    }
}
