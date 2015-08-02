using UnityEngine;
using System.Collections;

public class TravelBetweenTwoTransforms : MonoBehaviour
{
    public Transform start;
    public Transform mid;
    public Transform end;
    public Camera cam;

    public float current = 0;
    public float target = 1;
    float speed = 1;
    bool visible = true;
    bool firstTrip = true;
    void Start()
    {

    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (RaycastHit hit in Physics.RaycastAll(cam.ScreenPointToRay(Input.mousePosition)))
            {
                if(hit.collider.transform == transform)
                {
                    visible = !visible;
                    if (visible)
                    {
                        target = 1;
                    }
                    else
                    {
                        if(firstTrip)
                        {
                            firstTrip = false;
                            GameObject.Find("Receiver").GetComponent<LiftHandle>().DoYourThing();
                        }
                        target = 0;
                    }
                    break;
                }
            }
        }

        if (current != target)
        {
            if(target > current)
            {
                current += Time.deltaTime * speed;
                if(target < current)
                {
                    current = target;
                }
            }
            else
            {
                current -= Time.deltaTime * speed;
                if (target > current)
                {
                    current = target;
                }
            }

            transform.position = Vector3.Lerp(start.position, end.position, current);
            transform.position = Vector3.Lerp(transform.position, mid.position, Mathf.Sin(current * Mathf.PI) * 0.5f);
            transform.rotation = Quaternion.Lerp(start.rotation, end.rotation, current);
        }
    }
}
