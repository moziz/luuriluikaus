using UnityEngine;
using System.Collections.Generic;

public class ThrowableItem : MonoBehaviour
{
    bool inHand = true;
    bool inTheAir = false;
    bool inGround = false;

    public Vector2 startForceDirection = Vector2.zero;

    public Transform player;

    public Vector3 offset = new Vector3(-1.0f, 1.0f, 1.0f);

    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    void LateUpdate()
    {
        if (inHand)
        {
            transform.position = player.position + offset;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void GetThrown()
    {
        inHand = false;
        inTheAir = true;
        inGround = false;
        GetComponent<Rigidbody>().AddTorque(Vector3.forward * Random.Range(-100, -10));
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("THROWABLE COLLISION " + col.gameObject.name);

        if (col.gameObject.layer == LayerMask.NameToLayer("PlayerThrow"))
        {
            // Player collider hit
            if (inTheAir)
            {
                col.transform.parent.GetComponent<PlayerCharacter>().ThrowMe(GetComponent<Rigidbody>());
                GetComponent<Rigidbody>().AddTorque(Vector3.forward * Random.Range(-100, 100));
            }
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (inTheAir)
            {
                inHand = false;
                inTheAir = false;
                inGround = true;
            }
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }


    void OnCollisionEnter(Collision coll)
    {
        OnTriggerEnter(coll.collider);
    }
}