using UnityEngine;
using System.Collections;

public class KeepHeight : MonoBehaviour
{

    float height = 0;

    void Start()
    {
        height = transform.position.y;
    }
    
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = height;
        transform.position = pos;
    }
}
