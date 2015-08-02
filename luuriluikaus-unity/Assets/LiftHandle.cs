using UnityEngine;
using System.Collections;

public class LiftHandle : MonoBehaviour
{
    Vector3 startPos;
    Quaternion startRot;
    public Transform end;
    bool doingMyThing = false;
    float thingStartTime = 0;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }
    
    void Update()
    {
        if(doingMyThing)
        {
            transform.position = Vector3.Lerp(startPos, end.position, (Time.time - thingStartTime) * 1.0f);

            transform.rotation = Quaternion.Lerp(startRot, end.rotation, (Time.time - thingStartTime) * 1.0f);

            if (Time.time - thingStartTime > 2.0)
            {
                doingMyThing = false;
            }
        }
    }

    public void DoYourThing()
    {
        thingStartTime = Time.time + 1.0f;
        doingMyThing = true;
    }
}
