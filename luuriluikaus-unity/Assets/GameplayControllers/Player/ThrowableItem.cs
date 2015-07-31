using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    public Transform visuals;
    public float mass = 0.1f;
    public float drag = 0.0f;
    public float angularDrag = 0.05f;
    public Vector3 gravity = new Vector3(0, -1, 0);
}


public class ThrowableItem : MonoBehaviour
{
    bool inHand = true;
    bool inTheAir = false;
    bool inGround = false;

    public Vector2 startForceDirection = Vector2.zero;

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
        inHand = false;
        inTheAir = true;
        inGround = false;
        GetComponent<Rigidbody>().AddTorque(Vector3.forward * Random.Range(-100, -10));
    }

    void OnTriggerEnter(Collider col)
    {
        // Debug.Log("THROWABLE COLLISION " + col.gameObject.name);

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
            
            HitTheGround();
        }
    }


    void OnCollisionEnter(Collision coll)
    {
        OnTriggerEnter(coll.collider);
    }

    void HitTheGround()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        // TODO: Gameover

        PlayerCharacter p = GameObject.Find("Player").GetComponent<PlayerCharacter>();
        Debug.Log("GameOver");
        p.Stop();

        Destroy(this);
    }
}