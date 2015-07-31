using UnityEngine;
using System.Collections;

public class SpinZ : MonoBehaviour
{

    public float spinSpeed = -600.0f;
    public Quaternion origRot;
    ThrowableItem parent;

    void Start()
    {
        origRot = transform.rotation;
        parent = transform.parent.GetComponent<ThrowableItem>();
    }

    void Update()
    {
        transform.rotation = origRot;

        if (parent.inTheAir)
        {
            Vector3 rot = transform.eulerAngles;
            rot.z += spinSpeed * Time.time;
            transform.eulerAngles = rot;
        }
    }
}
