using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMissUI : MonoBehaviour {


    [SerializeField] private Transform leftContainer;
    [SerializeField] private Transform hitTemplate;
    [SerializeField] private Transform missTemplate;
    [SerializeField] private Transform blockedTemplate;
    [SerializeField] private Boxer boxer;


    private void Awake() {
        hitTemplate.gameObject.SetActive(false);
        missTemplate.gameObject.SetActive(false);
        blockedTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        boxer.OnSuccessfulAttack += Boxer_OnSuccessfulAttack;
        boxer.OnDodgedAttack += Boxer_OnDodgedAttack;
        boxer.OnBlockedAttack += Boxer_OnBlockedAttack;
    }


    private void Boxer_OnBlockedAttack(object sender, System.EventArgs e) {
        Transform newChild = Instantiate(blockedTemplate, leftContainer);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 2f);
    }


    private void Boxer_OnDodgedAttack(object sender, System.EventArgs e) {
        Transform newChild = Instantiate(missTemplate, leftContainer);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 2f);
    }

    private void Boxer_OnSuccessfulAttack(object sender, System.EventArgs e) {
        Transform newChild = Instantiate(hitTemplate, leftContainer);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 2f);
    }
}
