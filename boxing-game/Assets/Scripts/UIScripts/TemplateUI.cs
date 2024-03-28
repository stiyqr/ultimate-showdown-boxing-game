using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateUI : MonoBehaviour {


    private Vector2 startPos;
    private Vector2 targetPos;


    private void Start() {
        startPos = transform.position;
        targetPos.x = startPos.x;
        targetPos.y = startPos.y + 450;
    }

    private void Update() {
        startPos = transform.position;
        float lerpSpeed = .5f;
        transform.position = Vector2.Lerp(startPos, targetPos, Time.deltaTime * lerpSpeed);
    }
}
