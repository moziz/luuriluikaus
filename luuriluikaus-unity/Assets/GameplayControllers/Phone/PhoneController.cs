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
    public float rotaryRangeNumbersBegin = 0.05f;
    public float rotaryRangeNumbersEnd = 0.6f;
    public float rotaryRangeBegin = 0.1f;
    public float rotaryRangeEnd = 0.95f;
    public float rollbackSpeed = Mathf.PI / 32f;

    // State
    public RotaryState state {
        get;
        private set;
    }

    Collider rotaryCollider;
    Transform rotaryTrans;

    Quaternion originalOrientation;

    float rotateStart = 0f;
    float rotateReleaseTime = 0f;
    float rotaryReleaseDelta = 0f;
    float rotaryDelta = 0f;

    Vector2 inputCenterPoint;

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

        originalOrientation = rotaryTrans.rotation;

        state = RotaryState.rollback;
    }

    void Start() {

    }

    void Update() {
        inputCenterPoint = viewportCamera.WorldToScreenPoint(rotaryTrans.position);
        Vector2 inputPos = Input.mousePosition;
        float currentInputAngle = CalculateRotationAtPoint(inputPos);

        if (Input.GetMouseButtonDown(0) && state == RotaryState.rest) {
            // Start new rotation if hits

            Ray clickRay = viewportCamera.ScreenPointToRay(inputPos);
            RaycastHit rayHit;
            if (rotaryCollider.Raycast(clickRay, out rayHit, 10000f)) {
                // Start selecting
                rotateStart = currentInputAngle;
                state = RotaryState.selecting;
                
                if (event_rotaryStart != null) {
                    event_rotaryStart();
                }
            }
        }

        if (state == RotaryState.selecting) {
            float newDelta = currentInputAngle - rotateStart;

            if (newDelta > rotaryDelta) {
                rotaryDelta = currentInputAngle - rotateStart;

                // Prevent going over end
                if (rotaryDelta + rotateStart > rotaryRangeEnd) {
                    rotaryDelta = rotaryRangeEnd - rotateStart;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && state == RotaryState.selecting) {
            // Stop selecting

            // Check if the rotation is inside the number selection range
            
            rotaryReleaseDelta = rotaryDelta;
            rotateReleaseTime = Time.time;
            state = RotaryState.rollback;

            int rotaryOutputValue = -1;
            float numberRangeNormalized = (rotaryDelta - rotaryRangeNumbersBegin) / (rotaryRangeNumbersEnd - rotaryRangeNumbersBegin);
            rotaryOutputValue = (int)(numberRangeNormalized * rotaryDivision) + 1;

            if (rotaryOutputValue < 0 || rotaryOutputValue > rotaryDivision || rotaryReleaseDelta < rotaryRangeNumbersBegin || rotaryReleaseDelta > rotaryRangeNumbersEnd) {
                rotaryOutputValue = -1;
            } else if (rotaryOutputValue == rotaryDivision) {
                rotaryOutputValue = 0;
            }

            //Debug.Log("Rotaty released. releaseDelta: " + rotaryReleaseDelta + " norm: " + numberRangeNormalized + " Output value: " + rotaryOutputValue);
            Debug.Log("Phone dial released! Output: " + rotaryOutputValue);

            if (event_rotaryRelease != null) {
                event_rotaryRelease(rotaryOutputValue);
            }
        }
        
        if (state == RotaryState.rollback) {
            if (rotaryDelta > 0f) {
                rotaryDelta = (Time.time - rotateReleaseTime) * (-rollbackSpeed) + rotaryReleaseDelta;

            } else {
                rotaryDelta = 0f;

                state = RotaryState.rest;

                if (event_rotaryEnd != null) {
                    event_rotaryEnd();
                }
            }
        }

        // Update visualization
        if (state == RotaryState.selecting || state == RotaryState.rollback) {
            rotaryTrans.rotation = Quaternion.AngleAxis((rotaryDelta + rotaryRangeBegin) * 360f, originalOrientation * Vector3.up) * originalOrientation;
        }
    }

    float CalculateRotationAtPoint(Vector2 point) {
        Vector2 tanttarelli = point - inputCenterPoint;
        float angle = ((Mathf.Atan2(tanttarelli.x, tanttarelli.y) / Mathf.PI) + 1f) / 2f;

        return angle;
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
