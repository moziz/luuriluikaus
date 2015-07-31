using UnityEngine;
using System.Collections.Generic;

public class PhoneController : MonoBehaviour {
    public static PhoneController instance;

    public delegate void OnRotaryRelease(int rotaryValue);
    public delegate void OnRotaryStart();
    public delegate void OnRotaryEnd();

    public enum RotaryState {
        rest,
        selecting,
        rollback,
    }

    // Public references
    public GameObject rotaryObject;
    public Camera viewportCamera;

    // Settings 
    public int rotaryDivision = 9;
    public double rollbackSpeed = Mathf.PI / 16f;

    Collider rotaryCollider;
    Transform rotaryTrans;

    Quaternion originalOrientation;

    bool isRotating = false;
    float currentRotaryRotation = 0f;

    Vector2 inputCenterPoint;
    Vector2 inputPosLastFrame;

    OnRotaryStart event_rotaryStart;
    OnRotaryRelease event_rotaryRelease;
    OnRotaryEnd event_rotaryEnd;

    void Awake() {
        if (viewportCamera == null) {
            Debug.LogError("ViewportCamera is not set!");
            return;
        }

        if (viewportCamera == null) {
            Debug.LogError("Rotary GameObject is not set!");
            return;
        }

        instance = this;

        rotaryCollider = rotaryObject.GetComponent<Collider>();
        rotaryTrans = rotaryObject.GetComponent<Transform>();

        inputCenterPoint = viewportCamera.WorldToScreenPoint(rotaryTrans.position);

        originalOrientation = rotaryTrans.rotation;
    }

    void Start() {

    }

    void Update() {
        Vector2 clickPos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0) && !isRotating) {
            // Start new rotation if hits

            Ray clickRay = viewportCamera.ScreenPointToRay(clickPos);

            // Check hit
            RaycastHit rayHit;
            if (rotaryCollider.Raycast(clickRay, out rayHit, 10000f)) {
                // start rotating

                isRotating = true;
                inputPosLastFrame = clickPos;

                if (event_rotaryStart != null) {
                    event_rotaryStart();
                }

            } else {
                // Did not hit. Ignore
            }

        } else if (Input.GetMouseButtonUp(0) && isRotating) {
            // Stop rotating

            float rotaryValueF = ((currentRotaryRotation / Mathf.PI) + 1f) / 2f;
            int rotaryOutputValue = ((int)(rotaryValueF * rotaryDivision) + 1);

            if (rotaryOutputValue == rotaryDivision) {
                rotaryOutputValue = 0;
            }

            Debug.Log("Rotaty released. Output value: " + rotaryOutputValue);

            isRotating = false;

            if (event_rotaryRelease != null) {
                event_rotaryRelease(rotaryOutputValue);
            }

            // TODO Do this after rollback
            if (event_rotaryEnd != null) {
                event_rotaryEnd();
            }

        } else if (Input.GetMouseButton(0) && isRotating) {
            // Update rotation
            Vector2 tanttarelli = clickPos - inputCenterPoint;
            currentRotaryRotation = Mathf.Atan2(tanttarelli.x, tanttarelli.y);
            rotaryTrans.rotation = Quaternion.AngleAxis((currentRotaryRotation / (2f * Mathf.PI)) * 360f, originalOrientation * Vector3.up) * originalOrientation;

            inputPosLastFrame = clickPos;
        }
        
        if (!isRotating) {
            // Rollback

            // TODO Do this after rollback
            //if (event_rotaryEnd != null) {
            //  event_rotaryEnd();
            //}
        }
    }

    public void SubscribeOnRotaryRelease(OnRotaryRelease listener) {
        event_rotaryRelease += listener;
    }

    public void SubscribeOnRotaryStart(OnRotaryStart listener) {
        event_rotaryStart += listener;
    }

    public void SubscribeOnRotaryEnd(OnRotaryEnd listener) {
        event_rotaryEnd += listener;
    }
}
