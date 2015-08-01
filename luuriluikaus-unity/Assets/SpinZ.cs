using UnityEngine;
using System.Collections;

public class SpinZ : MonoBehaviour {
    public float spinSpeed = -600.0f;
    public Vector3 localRotationAxis = new Vector3(0f, 0f, 1f);
    public Quaternion origRot;
    ThrowableItem parent;

    void Start()
    {
        origRot = transform.rotation;
        parent = transform.parent.GetComponent<ThrowableItem>();
    }

    void Update()
    {
        if (parent.inTheAir) {
            transform.rotation = origRot * Quaternion.AngleAxis(spinSpeed * Time.time, localRotationAxis);
        }
    }
}
