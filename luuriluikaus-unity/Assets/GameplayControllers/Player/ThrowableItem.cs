using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    public string name;
    public Transform visuals;
    public float mass = 0.1f;
    public float drag = 0.0f;
    public float angularDrag = 0.05f;
    public float upwardForceMult = 1f;
    public float forwardForceMult = 1f;
    public Vector3 gravity = new Vector3(0, -1, 0);
}


public class ThrowableItem : MonoBehaviour
{
    public bool inHand = true;
    public bool inTheAir = false;
    public bool inGround = false;

    private bool gameEndedForMe = false;

    public float upwardForceMult = 1f;
    public float forwardForceMult = 1f;
    public Vector2 startForceDirection = Vector2.zero;

    public Transform obstaclePrefab;
    public Transform player;

    public Vector3 offset = new Vector3(-1.0f, 1.0f, 1.0f);

    public List<ItemData> items = new List<ItemData>();

    void Awake()
    {
        player = GameObject.Find("Player").transform;

        if (items.Count == 0) return;

        int itemIndex = Random.Range(0, items.Count);
        Rigidbody r = GetComponent<Rigidbody>();

        ItemData it = items[itemIndex];
        Instantiate<Transform>(it.visuals).parent = transform;
        r.mass = it.mass;
        r.drag = it.drag;
        Physics.gravity = it.gravity;
        upwardForceMult = it.upwardForceMult;
        forwardForceMult = it.forwardForceMult;

        inHand = true;
        inTheAir = false;
        inGround = false;
        gameEndedForMe = false;
        LateUpdate();
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
        // Get in front of player
        Vector3 pos = transform.position;
        pos.z = player.position.z - 2;
        transform.position = pos;

        inHand = false;
        inTheAir = true;
        inGround = false;
        GetComponent<Rigidbody>().AddTorque(Vector3.forward * Random.Range(-100, -10));
    }

    public void GetAbandoned()
    {
        gameEndedForMe = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("PlayerThrow"))
        {
            // Player collider hit
            if (inTheAir && !gameEndedForMe)
            {
                col.transform.parent.GetComponent<PlayerCharacter>().ThrowMe(GetComponent<Rigidbody>(), forwardForceMult, upwardForceMult);
                GetComponent<Rigidbody>().AddTorque(Vector3.forward * Random.Range(-100, 100));
            }
        }
        else if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HitTheGround();
        }
    }


    void OnCollisionEnter(Collision coll)
    {
        OnTriggerEnter(coll.collider);
    }

    void HitTheGround()
    {
        inHand = false;
        inTheAir = false;
        inGround = true;

        GetComponent<Rigidbody>().isKinematic = true;

        // TODO: Gameover
        if(!gameEndedForMe)
        {
            PlayerCharacter p = GameObject.Find("Player").GetComponent<PlayerCharacter>();
            Debug.Log("GameOver");
            p.Stop();
        }

        Instantiate<Transform>(obstaclePrefab).position = transform.position;

        Destroy(this);
    }
}