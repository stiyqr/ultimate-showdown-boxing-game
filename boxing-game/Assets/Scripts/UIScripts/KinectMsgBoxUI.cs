using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KinectMsgBoxUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI punchLText;
    [SerializeField] private TextMeshProUGUI punchRText;
    [SerializeField] private TextMeshProUGUI dodgeLText;
    [SerializeField] private TextMeshProUGUI dodgeRText;
    [SerializeField] private TextMeshProUGUI guardText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI kinectDetectedText;
    [SerializeField] private TextMeshProUGUI bodyframeDetectedText;
    [SerializeField] private TextMeshProUGUI gestureReadyText;
    [SerializeField] private TextMeshProUGUI gestureNotReadyText;

    [SerializeField] private Connector connector;


    private void Awake() {
        punchLText.gameObject.SetActive(false);
        punchRText.gameObject.SetActive(false);
        dodgeLText.gameObject.SetActive(false);
        dodgeRText.gameObject.SetActive(false);
        guardText.gameObject.SetActive(false);
        pauseText.gameObject.SetActive(false);
        kinectDetectedText.gameObject.SetActive(false);
        bodyframeDetectedText.gameObject.SetActive(false);
        gestureReadyText.gameObject.SetActive(false);
        gestureNotReadyText.gameObject.SetActive(false);
    }


    private void Start() {
        //GameInput.Instance.OnPlayerPunch_L += GameInput_OnPlayerPunch_L;
        //GameInput.Instance.OnPlayerPunch_R += GameInput_OnPlayerPunch_R;

/*        connector.OnKinectPunch_L += Connector_OnKinectPunch_L;
        connector.OnKinectPunch_R += Connector_OnKinectPunch_R;
        connector.OnKinectDodge_L += Connector_OnKinectDodge_L;
        connector.OnKinectDodge_R += Connector_OnKinectDodge_R;
        connector.OnKinectPauseAction += Connector_OnKinectPauseAction;*/
    }

    private void Connector_OnKinectPauseAction(object sender, System.EventArgs e) {
        Transform newChild = Instantiate(pauseText.transform, this.transform);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 1f);
    }

    private void Connector_OnKinectDodge_R(object sender, Connector.OnKinectDodge_REventArgs e) {
        Transform newChild = Instantiate(dodgeRText.transform, this.transform);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 1f);
    }

    private void Connector_OnKinectDodge_L(object sender, Connector.OnKinectDodge_LEventArgs e) {
        Transform newChild = Instantiate(dodgeLText.transform, this.transform);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 1f);
    }

    private void Connector_OnKinectPunch_R(object sender, System.EventArgs e) {
        Transform newChild = Instantiate(punchRText.transform, this.transform);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 1f);
    }

    private void Connector_OnKinectPunch_L(object sender, System.EventArgs e) {
        Transform newChild = Instantiate(punchLText.transform, this.transform);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 1f);
    }

    private void GameInput_OnPlayerPunch_R(object sender, System.EventArgs e) {
        Transform newChild = Instantiate(punchRText.transform, this.transform);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 1f);
    }

    private void GameInput_OnPlayerPunch_L(object sender, System.EventArgs e) {
        Transform newChild = Instantiate(punchLText.transform, this.transform);
        newChild.gameObject.SetActive(true);

        Destroy(newChild.gameObject, 1f);
    }
}
