using UnityEngine;
using System.Collections.Generic;

public class PhoneController : MonoBehaviour {
    public Transform rotaryTrans;

    public int rotaryDivision = 10;

    bool rotating = false;
    float rotatyValue = 0f;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // Start rotating

        } else if (Input.GetMouseButtonUp(0)) {
            // Stop rotating

        }
    }
}
